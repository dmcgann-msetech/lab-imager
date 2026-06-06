using LabImager.Models.Camera;

namespace LabImager.Services.Camera;

public interface ICameraDeviceService
{
    IReadOnlyList<CameraDeviceInfo> GetAvailableDevices();

    IReadOnlyList<CameraCaptureFormat> GetAvailableFormats(
        CameraDeviceInfo device);
}
