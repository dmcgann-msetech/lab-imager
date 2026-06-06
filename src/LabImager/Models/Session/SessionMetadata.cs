namespace LabImager.Models.Session;

public sealed class SessionMetadata
{
    public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedAt { get; init; } = DateTime.Now;

    public string ProjectName { get; set; } = string.Empty;
    public string BoardOrDevice { get; set; } = string.Empty;
    public string Technician { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string SourceDevicePath { get; set; } = string.Empty;
    public string NotesText { get; set; } = string.Empty;

    public string ApplicationName { get; init; } = "Lab Imager";
}
