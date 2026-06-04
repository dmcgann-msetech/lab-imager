using System;
using System.Windows.Media.Imaging;
using LabImager.Models.Camera;

namespace LabImager.Services.Preview;

public interface IWpfFramePreviewService
{
    bool IsPreviewRunning { get; }

    event EventHandler<BitmapSource>? FrameReady;

    void StartPreview(CameraDeviceInfo source);

    void StopPreview();
}
