using System.Windows;
using LabImager.Models.Session;

namespace LabImager.Services.Capture;

public interface IViewportCaptureService
{
    string CaptureElementToPng(FrameworkElement element);
    string CreateEvidencePng(string originalImagePath, SessionMetadata metadata);
}
