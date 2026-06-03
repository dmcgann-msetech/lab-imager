using DirectShowLib;
using LabImager.Models.Camera;

namespace LabImager.Services.Camera;

public sealed class DirectShowCameraDeviceService : ICameraDeviceService
{
public IReadOnlyList<CameraDeviceInfo> GetAvailableDevices()
{
var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

    return devices
        .Select(device => new CameraDeviceInfo
        {
            Name = device.Name,
            DevicePath = device.DevicePath
        })
        .ToList();
}

}
