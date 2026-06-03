using System.Windows;

namespace LabImager.Services.Capture;

public interface IViewportCaptureService
{
    string CaptureElementToPng(FrameworkElement element);
}
