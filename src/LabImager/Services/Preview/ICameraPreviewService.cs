using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public interface ICameraPreviewService
{
    bool IsPreviewRunning { get; }

    void StartPreview(CameraDeviceInfo source);

    void StopPreview();
}
