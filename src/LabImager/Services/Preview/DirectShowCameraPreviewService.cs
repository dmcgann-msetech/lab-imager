using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DirectShowLib;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public sealed class DirectShowCameraPreviewService : ICameraPreviewService
{
    private const WindowStyle PreviewWindowStyle =
        WindowStyle.Child |
        WindowStyle.ClipChildren;

    private IGraphBuilder? _graphBuilder;
    private ICaptureGraphBuilder2? _captureGraphBuilder;
    private IBaseFilter? _sourceFilter;
    private IBaseFilter? _smartTeeFilter;
    private IVideoWindow? _videoWindow;
    private IMediaControl? _mediaControl;
    private IBaseFilter? _recordingMuxFilter;
    private IFileSinkFilter? _recordingSinkFilter;

    private CameraDeviceInfo? _currentSource;
    private CameraCaptureFormat? _currentSelectedFormat;
    private IntPtr _currentPreviewHandle;
    private int _currentPreviewWidth;
    private int _currentPreviewHeight;
    private string? _pendingRecordingOutputPath;

    public bool IsPreviewRunning { get; private set; }

    public bool IsPreviewFrozen { get; private set; }

    public bool IsRecording { get; private set; }
    
    public void StartPreview(
        CameraDeviceInfo source,
        CameraCaptureFormat? selectedFormat,
        IntPtr previewHandle,
        int width,
        int height)
    {
        if (string.IsNullOrWhiteSpace(source.DevicePath) && string.IsNullOrWhiteSpace(source.Name))
        {
            throw new ArgumentException("Cannot start preview without a valid source.", nameof(source));
        }

        _currentSource = source;
        _currentSelectedFormat = selectedFormat;
        _currentPreviewHandle = previewHandle;
        _currentPreviewWidth = width;
        _currentPreviewHeight = height;

        StopPreview();

        var device = DsDevice
            .GetDevicesOfCat(FilterCategory.VideoInputDevice)
            .FirstOrDefault(candidate =>
                string.Equals(candidate.DevicePath, source.DevicePath, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(candidate.Name, source.Name, StringComparison.OrdinalIgnoreCase));

        if (device == null)
        {
            throw new InvalidOperationException($"DirectShow source not found: {source.Name}");
        }

        _graphBuilder = (IGraphBuilder)new FilterGraph();
        _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        var hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
        DsError.ThrowExceptionForHR(hr);

        IsPreviewRunning = true;
        IsPreviewFrozen = false;
        IsPreviewFrozen = false;
        object sourceObject;
        var filterGuid = typeof(IBaseFilter).GUID;

        device.Mon.BindToObject(null!, null!, ref filterGuid, out sourceObject);

        _sourceFilter = (IBaseFilter)sourceObject;

        hr = _graphBuilder.AddFilter(_sourceFilter, source.Name);
        DsError.ThrowExceptionForHR(hr);

        _smartTeeFilter = (IBaseFilter)new SmartTee();
        hr = _graphBuilder.AddFilter(_smartTeeFilter, "Smart Tee");
        DsError.ThrowExceptionForHR(hr);

        ApplySelectedFormat(
            _captureGraphBuilder,
            _sourceFilter,
            selectedFormat);

        hr = _captureGraphBuilder.RenderStream(
            PinCategory.Preview,
            MediaType.Video,
            _sourceFilter,
            _smartTeeFilter,
            null);

        DsError.ThrowExceptionForHR(hr);

        if (!string.IsNullOrWhiteSpace(_pendingRecordingOutputPath))
        {
            var outputDirectory = Path.GetDirectoryName(_pendingRecordingOutputPath);

            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            hr = _captureGraphBuilder.SetOutputFileName(
                MediaSubType.Avi,
                _pendingRecordingOutputPath,
                out _recordingMuxFilter,
                out _recordingSinkFilter);

            if (hr < 0)
            {
                throw new InvalidOperationException($"SetOutputFileName failed. HRESULT: 0x{hr:X8}");
            }

            // Build recording branch before graph run.
            hr = _captureGraphBuilder.RenderStream(
                PinCategory.Capture,
                MediaType.Video,
                _sourceFilter,
                null,
                _recordingMuxFilter);

            if (hr < 0)
            {
                throw new InvalidOperationException($"RenderStream capture-to-AVI failed. HRESULT: 0x{hr:X8}");
            }
        }

        _videoWindow = _graphBuilder as IVideoWindow;
        _mediaControl = _graphBuilder as IMediaControl;

        if (_videoWindow == null || _mediaControl == null)
        {
            throw new InvalidOperationException("DirectShow preview graph did not expose required control interfaces.");
        }

        hr = _videoWindow.put_Owner(previewHandle);
        DsError.ThrowExceptionForHR(hr);

        hr = _videoWindow.put_WindowStyle(PreviewWindowStyle);
        DsError.ThrowExceptionForHR(hr);

        ResizePreview(width, height);

        hr = _videoWindow.put_Visible(OABool.True);
        DsError.ThrowExceptionForHR(hr);

        hr = _mediaControl.Run();
        DsError.ThrowExceptionForHR(hr);

        IsPreviewRunning = true;
        IsPreviewFrozen = false;
    }

    public void ResizePreview(int width, int height)
    {
        if (_videoWindow == null || width <= 0 || height <= 0)
        {
            return;
        }

        var hr = _videoWindow.SetWindowPosition(0, 0, width, height);
        DsError.ThrowExceptionForHR(hr);
    }

    public void FreezePreview()
    {
        if (_mediaControl == null || !IsPreviewRunning || IsPreviewFrozen)
        {
            return;
        }

        var hr = _mediaControl.Pause();
        DsError.ThrowExceptionForHR(hr);

        IsPreviewFrozen = true;
    }

    public void ResumePreview()
    {
        if (_mediaControl == null || !IsPreviewRunning || !IsPreviewFrozen)
        {
            return;
        }

        var hr = _mediaControl.Run();
        DsError.ThrowExceptionForHR(hr);

        IsPreviewFrozen = false;
    }

    public void StopPreview()
    {
        if (_mediaControl != null)
        {
            try
            {
                _mediaControl.Stop();
            }
            catch
            {
                // Ignore stop failures during cleanup.
            }
        }

        if (_videoWindow != null)
        {
            try
            {
                _videoWindow.put_Visible(OABool.False);
                _videoWindow.put_Owner(IntPtr.Zero);
            }
            catch
            {
                // Ignore window cleanup failures during teardown.
            }
        }

        ReleaseComObject(_mediaControl);
        ReleaseComObject(_videoWindow);
        ReleaseComObject(_sourceFilter);
        ReleaseComObject(_captureGraphBuilder);
        ReleaseComObject(_graphBuilder);

        _mediaControl = null;
        _videoWindow = null;
        _sourceFilter = null;
        _captureGraphBuilder = null;
        _graphBuilder = null;

        IsPreviewRunning = false;
        IsPreviewFrozen = false;
    }

    public void StartRecording(string outputPath)
    {
        if (IsRecording)
        {
            return;
        }

        if (!IsPreviewRunning ||
            _currentSource == null ||
            _currentPreviewHandle == IntPtr.Zero ||
            _currentPreviewWidth <= 0 ||
            _currentPreviewHeight <= 0)
        {
            throw new InvalidOperationException("Preview must be running before recording can start.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Recording output path is required.", nameof(outputPath));
        }

        _pendingRecordingOutputPath = outputPath;

        try
        {
            StartPreview(
                _currentSource,
                _currentSelectedFormat,
                _currentPreviewHandle,
                _currentPreviewWidth,
                _currentPreviewHeight);

            if (!File.Exists(outputPath))
            {
                throw new InvalidOperationException($"Recording graph started, but output file was not created: {outputPath}");
            }

            IsRecording = true;
        }
        catch
        {
            _pendingRecordingOutputPath = null;
            IsRecording = false;

            if (_currentSource != null && _currentPreviewHandle != IntPtr.Zero)
            {
                try
                {
                    StartPreview(
                        _currentSource,
                        _currentSelectedFormat,
                        _currentPreviewHandle,
                        _currentPreviewWidth,
                        _currentPreviewHeight);
                }
                catch
                {
                    // Ignore preview restore failure so original recording error is preserved.
                }
            }

            throw;
        }
    }

    public void StopRecording()
    {
        if (!IsRecording)
        {
            return;
        }

        _pendingRecordingOutputPath = null;
        IsRecording = false;

        if (_currentSource == null || _currentPreviewHandle == IntPtr.Zero)
        {
            StopPreview();
            return;
        }

        StartPreview(
            _currentSource,
            _currentSelectedFormat,
            _currentPreviewHandle,
            _currentPreviewWidth,
            _currentPreviewHeight);
    }

    private static void ApplySelectedFormat(
        ICaptureGraphBuilder2 captureGraphBuilder,
        IBaseFilter sourceFilter,
        CameraCaptureFormat? selectedFormat)
    {
        if (selectedFormat == null || !selectedFormat.IsAvailable)
        {
            return;
        }

        var streamConfigGuid = typeof(IAMStreamConfig).GUID;

        var hr = captureGraphBuilder.FindInterface(
            PinCategory.Capture,
            MediaType.Video,
            sourceFilter,
            streamConfigGuid,
            out var configObject);

        if (hr < 0)
        {
            hr = captureGraphBuilder.FindInterface(
                PinCategory.Preview,
                MediaType.Video,
                sourceFilter,
                streamConfigGuid,
                out configObject);
        }

        DsError.ThrowExceptionForHR(hr);

        var streamConfig = (IAMStreamConfig)configObject;
        IntPtr capsPointer = IntPtr.Zero;

        try
        {
            DsError.ThrowExceptionForHR(
                streamConfig.GetNumberOfCapabilities(
                    out var count,
                    out var size));

            capsPointer = Marshal.AllocHGlobal(size);

            for (var i = 0; i < count; i++)
            {
                AMMediaType? mediaType = null;

                try
                {
                    DsError.ThrowExceptionForHR(
                        streamConfig.GetStreamCaps(
                            i,
                            out mediaType,
                            capsPointer));

                    if (mediaType == null ||
                        mediaType!.formatPtr == IntPtr.Zero ||
                        mediaType!.formatType != FormatType.VideoInfo)
                    {
                        continue;
                    }

                    var info = Marshal.PtrToStructure<VideoInfoHeader>(
                        mediaType!.formatPtr);

                    var width = info.BmiHeader.Width;
                    var height = Math.Abs(info.BmiHeader.Height);
                    var fps = info.AvgTimePerFrame > 0
                        ? Math.Round(10000000.0 / info.AvgTimePerFrame, 2)
                        : 0;

                    var subtypeName = GetSubtypeName(mediaType!.subType);

                    if (!FormatMatches(selectedFormat, width, height, fps, subtypeName))
                    {
                        continue;
                    }

                    DsError.ThrowExceptionForHR(
                        streamConfig.SetFormat(mediaType!));

                    return;
                }
                finally
                {
                    if (mediaType != null)
                    {
                        DsUtils.FreeAMMediaType(mediaType);
                    }
                }
            }
        }
        finally
        {
            if (capsPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(capsPointer);
            }

            ReleaseComObject(streamConfig);
        }
    }

    private static bool FormatMatches(
        CameraCaptureFormat selectedFormat,
        int width,
        int height,
        double fps,
        string subtypeName)
    {
        return selectedFormat.Width == width &&
               selectedFormat.Height == height &&
               Math.Abs(selectedFormat.FramesPerSecond - fps) < 0.5 &&
               string.Equals(selectedFormat.SubType, subtypeName, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetSubtypeName(Guid subtype)
    {
        var subtypeMap = new Dictionary<Guid, string>
        {
            [MediaSubType.MJPG] = "MJPG",
            [MediaSubType.YUY2] = "YUY2",
            [MediaSubType.NV12] = "NV12",
            [FourCcToGuid("H264")] = "H264",
            [FourCcToGuid("H265")] = "H265",
            [FourCcToGuid("HEVC")] = "HEVC",
            [FourCcToGuid("I420")] = "I420",
            [FourCcToGuid("YV12")] = "YV12"
        };

        return subtypeMap.TryGetValue(subtype, out var name)
            ? name
            : subtype.ToString();
    }

    private static Guid FourCcToGuid(string fourCc)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes(fourCc);
        var value = BitConverter.ToUInt32(bytes, 0);

        return new Guid(
            (int)value,
            0x0000,
            0x0010,
            0x80,
            0x00,
            0x00,
            0xaa,
            0x00,
            0x38,
            0x9b,
            0x71);
    }

    private static void ReleaseComObject(object? comObject)
    {
        if (comObject == null)
        {
            return;
        }

        if (Marshal.IsComObject(comObject))
        {
            Marshal.ReleaseComObject(comObject);
        }
    }
}











