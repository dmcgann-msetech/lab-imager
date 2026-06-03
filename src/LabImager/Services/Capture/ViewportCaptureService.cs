using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabImager.Services.Capture;

public sealed class ViewportCaptureService : IViewportCaptureService
{
    public string CaptureElementToPng(FrameworkElement element)
    {
        if (element.ActualWidth <= 0 || element.ActualHeight <= 0)
        {
            throw new InvalidOperationException("Viewport has no rendered size to capture.");
        }

        var capturesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "Lab Imager",
            "Captures");

        Directory.CreateDirectory(capturesDirectory);

        var fileName = $"lab-imager-capture-{DateTime.Now:yyyyMMdd-HHmmss}.png";
        var filePath = Path.Combine(capturesDirectory, fileName);

        var width = (int)Math.Ceiling(element.ActualWidth);
        var height = (int)Math.Ceiling(element.ActualHeight);

        var renderTarget = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);

        renderTarget.Render(element);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderTarget));

        using var stream = File.Create(filePath);
        encoder.Save(stream);

        return filePath;
    }
}
