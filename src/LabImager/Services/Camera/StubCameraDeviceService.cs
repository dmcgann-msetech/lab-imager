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
}
