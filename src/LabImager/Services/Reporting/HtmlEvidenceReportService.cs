using System;
using System.IO;
using System.Text;
using LabImager.Models.Capture;

namespace LabImager.Services.Reporting;

public sealed class HtmlEvidenceReportService
    : IHtmlEvidenceReportService
{
    public string GenerateReport(
        CaptureRecord captureRecord,
        string outputDirectory)
    {
        var reportPath = Path.Combine(
            outputDirectory,
            $"{captureRecord.CaptureId}-report.html");

        var html = """
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>Lab Imager Evidence Report</title>
</head>
<body>

<h1>Lab Imager Evidence Report</h1>

<p><strong>Project:</strong> PROJECT</p>
<p><strong>Board / Device:</strong> BOARD</p>
<p><strong>Technician:</strong> TECHNICIAN</p>

</body>
</html>
""";

        html = html.Replace(
            "PROJECT",
            captureRecord.Session.ProjectName);

        html = html.Replace(
            "BOARD",
            captureRecord.Session.BoardOrDevice);

        html = html.Replace(
            "TECHNICIAN",
            captureRecord.Session.Technician);

        File.WriteAllText(
            reportPath,
            html,
            Encoding.UTF8);

        return reportPath;
    }
}
