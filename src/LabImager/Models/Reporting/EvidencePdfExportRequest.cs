using System;

namespace LabImager.Models.Reporting
{
    public sealed class EvidencePdfExportRequest
    {
        public string EvidenceImagePath { get; init; } = string.Empty;
        public string OutputPdfPath { get; init; } = string.Empty;

        public string ProjectName { get; init; } = string.Empty;
        public string BoardOrDevice { get; init; } = string.Empty;
        public string Technician { get; init; } = string.Empty;
        public string SourceName { get; init; } = string.Empty;
        public string SourceDevicePath { get; init; } = string.Empty;
        public string CaptureFormat { get; init; } = string.Empty;
        public string NotesText { get; init; } = string.Empty;

        public DateTime CapturedAt { get; init; }
        public DateTime ExportedAt { get; init; } = DateTime.Now;
    }
}
