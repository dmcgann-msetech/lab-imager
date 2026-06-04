using System;
using System.Windows.Media.Imaging;
using DirectShowLib;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public sealed class DirectShowFramePreviewService : IWpfFramePreviewService, ISampleGrabberCB
{
    public bool IsPreviewRunning { get; private set; }

    public event EventHandler<BitmapSource>? FrameReady;

    public void StartPreview(CameraDeviceInfo source)
    {
        if (string.IsNullOrWhiteSpace(source.DevicePath) && string.IsNullOrWhiteSpace(source.Name))
        {
            throw new ArgumentException("Cannot start frame preview without a valid source.", nameof(source));
        }

        StopPreview();

        // M4-B scaffold only.
        // The next milestone will build the DirectShow graph using SampleGrabber.
        IsPreviewRunning = true;
    }

    public void StopPreview()
    {
        IsPreviewRunning = false;
    }

    public int SampleCB(double sampleTime, IMediaSample mediaSample)
    {
        return 0;
    }

    public int BufferCB(double sampleTime, IntPtr buffer, int bufferLength)
    {
        return 0;
    }

    private void RaiseFrameReady(BitmapSource frame)
    {
        FrameReady?.Invoke(this, frame);
    }
}
