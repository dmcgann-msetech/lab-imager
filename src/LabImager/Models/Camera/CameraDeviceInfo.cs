namespace LabImager.Models.Camera;

public sealed class CameraDeviceInfo
{
public string Name { get; init; } = string.Empty;

public string DevicePath { get; init; } = string.Empty;

public string Description
{
    get
    {
        if (string.IsNullOrWhiteSpace(DevicePath))
        {
            return Name;
        }

        return $"{Name} ({DevicePath})";
    }
}

}
