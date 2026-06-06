namespace LabImager.Models.Camera;

public sealed class CameraCaptureFormat
{
    public string DisplayName { get; init; } = string.Empty;

    public int Width { get; init; }

    public int Height { get; init; }

    public double FramesPerSecond { get; init; }

    public string SubType { get; init; } = string.Empty;

    public bool IsAvailable { get; init; }

    public override string ToString()
    {
        return DisplayName;
    }
}
