using LabImager.Models.Reporting;

namespace LabImager.Services.Reporting
{
    public interface IEvidencePdfExportService
    {
        string Export(EvidencePdfExportRequest request);
    }
}
