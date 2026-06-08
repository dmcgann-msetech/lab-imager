using LabImager.Models.Capture;

namespace LabImager.Services.Reporting;

public interface IHtmlEvidenceReportService
{
    string GenerateReport(
        CaptureRecord captureRecord,
        string outputDirectory);
}
