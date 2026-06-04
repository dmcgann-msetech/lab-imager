using System;
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
    private IVideoWindow? _videoWindow;
    private IMediaControl? _mediaControl;

    public bool IsPreviewRunning { get; private set; }

    public void StartPreview(CameraDeviceInfo source, IntPtr previewHandle, int width, int height)
    {
        if (string.IsNullOrWhiteSpace(source.DevicePath) && string.IsNullOrWhiteSpace(source.Name))
        {
            throw new ArgumentException("Cannot start preview without a valid source.", nameof(source));
        }

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

        object sourceObject;
        var filterGuid = typeof(IBaseFilter).GUID;

        device.Mon.BindToObject(null!, null!, ref filterGuid, out sourceObject);

        _sourceFilter = (IBaseFilter)sourceObject;

        hr = _graphBuilder.AddFilter(_sourceFilter, source.Name);
        DsError.ThrowExceptionForHR(hr);

        hr = _captureGraphBuilder.RenderStream(
            PinCategory.Preview,
            MediaType.Video,
            _sourceFilter,
            null,
            null);

        DsError.ThrowExceptionForHR(hr);

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

