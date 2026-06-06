using LabImager.Models.Session;

namespace LabImager.Models.Capture;

public sealed class CaptureRecord
{
    public string CaptureId { get; init; } = Guid.NewGuid().ToString("N");
    public DateTime CapturedAt { get; init; } = DateTime.Now;

    public required SessionMetadata Session { get; init; }

    public string OriginalImagePath { get; set; } = string.Empty;
    public string AnnotatedImagePath { get; set; } = string.Empty;
    public string ReportPath { get; set; } = string.Empty;

    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
}
