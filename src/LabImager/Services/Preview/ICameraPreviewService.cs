using System;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public interface ICameraPreviewService
{
    bool IsPreviewRunning { get; }

    bool IsPreviewFrozen { get; }

    bool IsRecording { get; }

    void StartPreview(
        CameraDeviceInfo source,
        CameraCaptureFormat? selectedFormat,
        IntPtr previewHandle,
        int width,
        int height);

    void ResizePreview(int width, int height);

    void FreezePreview();

    void ResumePreview();

    void StopPreview();

    void StartRecording(string outputPath);

    void StopRecording();
}
