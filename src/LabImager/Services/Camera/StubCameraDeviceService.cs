using LabImager.Models.Camera;

namespace LabImager.Services.Camera;

public sealed class StubCameraDeviceService : ICameraDeviceService
{
    public IReadOnlyList<CameraDeviceInfo> GetAvailableDevices()
    {
        return
        [
            new CameraDeviceInfo
            {
                Name = "Camera enumeration not connected yet",
                DevicePath = "Phase 2 stub"
            }
        ];
    }

    public IReadOnlyList<CameraCaptureFormat> GetAvailableFormats(
        CameraDeviceInfo device)
    {
        return
        [
            new CameraCaptureFormat
            {
                DisplayName = "Format enumeration not connected yet",
                Width = 0,
                Height = 0,
                FramesPerSecond = 0,
                SubType = "Stub",
                IsAvailable = false
            }
        ];
    }
}
