using System;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public sealed class StubCameraPreviewService : ICameraPreviewService
{
    public bool IsPreviewRunning { get; private set; }

    public CameraDeviceInfo? ActiveSource { get; private set; }

    public void StartPreview(CameraDeviceInfo source)
    {
        if (string.IsNullOrWhiteSpace(source.Name))
        {
            throw new ArgumentException("Cannot start preview without a valid source name.", nameof(source));
        }

        ActiveSource = source;
        IsPreviewRunning = true;
    }

    public void StopPreview()
    {
        ActiveSource = null;
        IsPreviewRunning = false;
    }
}
