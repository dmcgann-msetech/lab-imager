using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LabImager.Models.Session;

namespace LabImager.Services.Capture;

public sealed class ViewportCaptureService : IViewportCaptureService
{
    public string CaptureElementToPng(FrameworkElement element)
    {
        if (element.ActualWidth <= 0 || element.ActualHeight <= 0)
            throw new InvalidOperationException("Viewport has no rendered size to capture.");

        var capturesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "Lab Imager",
            "Captures");

        Directory.CreateDirectory(capturesDirectory);

        var fileName = $"lab-imager-capture-{DateTime.Now:yyyyMMdd-HHmmss}-original.png";
        var filePath = Path.Combine(capturesDirectory, fileName);

        var width = (int)Math.Ceiling(element.ActualWidth);
        var height = (int)Math.Ceiling(element.ActualHeight);

        var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        renderTarget.Render(element);

        SaveBitmap(renderTarget, filePath);
        return filePath;
    }

    public string CreateEvidencePng(string originalImagePath, SessionMetadata metadata)
    {
        if (!File.Exists(originalImagePath))
            throw new FileNotFoundException("Original capture image was not found.", originalImagePath);

        var source = LoadBitmap(originalImagePath);
        var width = source.PixelWidth;
        var imageHeight = source.PixelHeight;

        var metadataLines = BuildMetadataLines(metadata);
        var panelHeight = Math.Max(260, 34 + metadataLines.Count * 24);

        var visual = new DrawingVisual();

        using (var context = visual.RenderOpen())
        {
            context.DrawRectangle(System.Windows.Media.Brushes.White, null, new Rect(0, 0, width, imageHeight + panelHeight));
            context.DrawImage(source, new Rect(0, 0, width, imageHeight));

            var separatorPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.LightGray, 2);
            context.DrawLine(separatorPen, new System.Windows.Point(0, imageHeight), new System.Windows.Point(width, imageHeight));

            var y = imageHeight + 22;

            foreach (var line in metadataLines)
            {
                var text = new FormattedText(
                    line,
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    16,
                    System.Windows.Media.Brushes.Black,
                    1.0);

                context.DrawText(text, new System.Windows.Point(24, y));
                y += 24;
            }
        }

        var output = new RenderTargetBitmap(width, imageHeight + panelHeight, 96, 96, PixelFormats.Pbgra32);
        output.Render(visual);

        var evidencePath = Path.Combine(
            Path.GetDirectoryName(originalImagePath)!,
            Path.GetFileNameWithoutExtension(originalImagePath).Replace("-original", "-evidence") + ".png");

        SaveBitmap(output, evidencePath);
        return evidencePath;
    }

    private static BitmapImage LoadBitmap(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path);
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private static void SaveBitmap(BitmapSource bitmap, string path)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        using var stream = File.Create(path);
        encoder.Save(stream);
    }

    private static List<string> BuildMetadataLines(SessionMetadata metadata)
    {
        var lines = new List<string>
        {
            $"Project: {metadata.ProjectName}",
            $"Board / Device: {metadata.BoardOrDevice}",
            $"Technician: {metadata.Technician}",
            $"Source: {metadata.SourceName}",
            $"Captured: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            "Notes:"
        };

        var notes = string.IsNullOrWhiteSpace(metadata.NotesText)
            ? "(none)"
            : metadata.NotesText.Trim();

        foreach (var line in Wrap(notes, 96))
            lines.Add($"  {line}");

        return lines;
    }

    private static IEnumerable<string> Wrap(string text, int maxLength)
    {
        foreach (var rawLine in text.Replace("\r", "").Split('\n'))
        {
            var line = rawLine.Trim();

            while (line.Length > maxLength)
            {
                var breakAt = line.LastIndexOf(' ', maxLength);
                if (breakAt <= 0)
                    breakAt = maxLength;

                yield return line[..breakAt].Trim();
                line = line[breakAt..].Trim();
            }

            if (line.Length > 0)
                yield return line;
        }
    }
}

