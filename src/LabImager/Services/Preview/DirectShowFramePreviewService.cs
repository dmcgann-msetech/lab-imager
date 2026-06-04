using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using DirectShowLib;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public sealed class DirectShowFramePreviewService : IWpfFramePreviewService, ISampleGrabberCB
{
    private IGraphBuilder? _graphBuilder;
    private ICaptureGraphBuilder2? _captureGraphBuilder;
    private IBaseFilter? _sourceFilter;
    private IBaseFilter? _sampleGrabberFilter;
    private IBaseFilter? _nullRenderer;
    private IMediaControl? _mediaControl;
    private ISampleGrabber? _sampleGrabber;

    public bool IsPreviewRunning { get; private set; }

    public event EventHandler<BitmapSource>? FrameReady;

    private System.Windows.Media.Imaging.BitmapSource? _latestFrame;

    private int _frameCount = 0;
private DateTime _lastLogTime = DateTime.Now;

    public void StartPreview(CameraDeviceInfo source)
    {
        if (string.IsNullOrWhiteSpace(source.DevicePath) &&
            string.IsNullOrWhiteSpace(source.Name))
        {
            throw new ArgumentException(
                "Cannot start frame preview without a valid source.",
                nameof(source));
        }

        StopPreview();

        var device = DsDevice
            .GetDevicesOfCat(FilterCategory.VideoInputDevice)
            .FirstOrDefault(candidate =>
                string.Equals(candidate.DevicePath,
                    source.DevicePath,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(candidate.Name,
                    source.Name,
                    StringComparison.OrdinalIgnoreCase));

        if (device == null)
        {
            throw new InvalidOperationException(
                $"DirectShow source not found: {source.Name}");
        }

        _graphBuilder = (IGraphBuilder)new FilterGraph();
        _captureGraphBuilder =
            (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

        DsError.ThrowExceptionForHR(
            _captureGraphBuilder.SetFiltergraph(_graphBuilder));

        object sourceObject;
        var filterGuid = typeof(IBaseFilter).GUID;

        device.Mon.BindToObject(
            null!,
            null!,
            ref filterGuid,
            out sourceObject);

        _sourceFilter = (IBaseFilter)sourceObject;

        DsError.ThrowExceptionForHR(
            _graphBuilder.AddFilter(_sourceFilter, "Source"));

        _sampleGrabberFilter =
            (IBaseFilter)new SampleGrabber();

        _sampleGrabber =
            (ISampleGrabber)_sampleGrabberFilter;

        _nullRenderer =
            (IBaseFilter)new NullRenderer();

        DsError.ThrowExceptionForHR(
            _graphBuilder.AddFilter(
                _sampleGrabberFilter,
                "SampleGrabber"));

        DsError.ThrowExceptionForHR(
            _graphBuilder.AddFilter(
                _nullRenderer,
                "NullRenderer"));

        var mediaType = new AMMediaType
        {
            majorType = MediaType.Video,
            subType = MediaSubType.RGB24,
            formatType = FormatType.VideoInfo
        };

        _sampleGrabber.SetMediaType(mediaType);
        _sampleGrabber.SetBufferSamples(false);
        _sampleGrabber.SetOneShot(false);
        _sampleGrabber.SetCallback(this, 1);

        DsError.ThrowExceptionForHR(
            _captureGraphBuilder.RenderStream(
                PinCategory.Preview,
                MediaType.Video,
                _sourceFilter,
                _sampleGrabberFilter,
                _nullRenderer));

        _mediaControl = (IMediaControl)_graphBuilder;

        DsError.ThrowExceptionForHR(
            _mediaControl.Run());

        IsPreviewRunning = true;
    }

    public void StopPreview()
    {
        try
        {
            _mediaControl?.Stop();
        }
        catch
        {
        }

        Release(_mediaControl);
        Release(_sampleGrabber);
        Release(_nullRenderer);
        Release(_sampleGrabberFilter);
        Release(_sourceFilter);
        Release(_captureGraphBuilder);
        Release(_graphBuilder);

        _mediaControl = null;
        _sampleGrabber = null;
        _nullRenderer = null;
        _sampleGrabberFilter = null;
        _sourceFilter = null;
        _captureGraphBuilder = null;
        _graphBuilder = null;

        IsPreviewRunning = false;
    }

    public int SampleCB(double sampleTime, IMediaSample mediaSample)
    {
        return 0;
    }

    public int BufferCB(double sampleTime, IntPtr buffer, int bufferLength)
{
    try
    {
        if (buffer == IntPtr.Zero || bufferLength <= 0)
            return 0;

        byte[] managedBuffer = new byte[bufferLength];
        Marshal.Copy(buffer, managedBuffer, 0, bufferLength);

        // SAFE: Do NOT assume dimensions yet
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            // Temporary diagnostic frame signal only
            FrameReady?.Invoke(this, null!);
        });
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("[FRAME ERROR] " + ex.Message);
    }

    return 0;
}

private void FrameHeartbeat()
{
    System.Diagnostics.Debug.WriteLine("[ANALYSIS MODE] Heartbeat OK");
}

    private void RaiseFrameReady(BitmapSource frame)
{
    System.Windows.Application.Current?.Dispatcher.Invoke(() =>
    {
        if (frame == null) return;
        FrameReady?.Invoke(this, frame);
    });
}

    private static void Release(object? obj)
    {
        if (obj != null && Marshal.IsComObject(obj))
        {
            Marshal.ReleaseComObject(obj);
        }
    }
}






