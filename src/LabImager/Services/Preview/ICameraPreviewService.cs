using System;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public interface ICameraPreviewService
{
    bool IsPreviewRunning { get; }

    bool IsPreviewFrozen { get; }

    void StartPreview(CameraDeviceInfo source, IntPtr previewHandle, int width, int height);

    void ResizePreview(int width, int height);

    void FreezePreview();

    void ResumePreview();

    void StopPreview();
}
