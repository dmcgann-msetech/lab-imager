using System;

namespace LabImager.Models.Reporting
{
    public sealed class LatestEvidenceCapture
    {
        public string OriginalImagePath { get; init; } = string.Empty;
        public string EvidenceImagePath { get; init; } = string.Empty;
        public DateTime CapturedAt { get; init; }
    }
}
