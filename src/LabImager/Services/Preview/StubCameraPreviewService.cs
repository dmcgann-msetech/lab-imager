using System;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public sealed class StubCameraPreviewService : ICameraPreviewService
{
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
        IsPreviewRunning = true;
        IsPreviewFrozen = false;
    }

    public void ResizePreview(int width, int height)
    {
    }

    public void FreezePreview()
    {
        if (!IsPreviewRunning)
        {
            return;
        }

        IsPreviewFrozen = true;
    }

    public void ResumePreview()
    {
        if (!IsPreviewRunning)
        {
            return;
        }

        IsPreviewFrozen = false;
    }

    public void StopPreview()
    {
        IsPreviewRunning = false;
        IsPreviewFrozen = false;
        IsRecording = false;
    }

    public void StartRecording(string outputPath)
    {
        IsRecording = true;
    }

    public void StopRecording()
    {
        IsRecording = false;
    }
}
