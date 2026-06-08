using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using LabImager.Services.Camera;
using LabImager.Services.Capture;
using LabImager.Models.Session;
using LabImager.Models.Camera;
using LabImager.Services.Preview;
using LabImager.Services.Recording;
using LabImager.Services.Settings;

namespace LabImager
{
    public partial class MainWindow : Window
    {
        private readonly ICameraDeviceService _cameraDeviceService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IViewportCaptureService _viewportCaptureService;
        private readonly ICameraPreviewService _cameraPreviewService;
        private readonly IRecordingService _recordingService;
        private TextBlock? _recordingHeaderText;

        public MainWindow()
        {
            InitializeComponent();

            _cameraDeviceService = new DirectShowCameraDeviceService();
            _appSettingsService = new JsonAppSettingsService();
            _viewportCaptureService = new ViewportCaptureService();
            _cameraPreviewService = new DirectShowCameraPreviewService();
            _recordingService = new RecordingService();

            _recordingService.StateChanged += (_, _) => UpdateRecordingUi();
            _recordingService.ElapsedChanged += (_, _) => UpdateRecordingUi();

            PreviewPanel.Resize += (_, _) =>
            {
                if (_cameraPreviewService.IsPreviewRunning)
                {
                    _cameraPreviewService.ResizePreview(
                        PreviewPanel.ClientSize.Width,
                        PreviewPanel.ClientSize.Height);
                }
            };

            StopPreviewButton.IsEnabled = false;

            LoadCameraDevices();
        }

        private void LoadCameraDevices()
        {
            CameraSelector.Items.Clear();

            var settings = _appSettingsService.Load();
            var devices = _cameraDeviceService.GetAvailableDevices();

            if (devices == null || devices.Count == 0)
            {
                CameraSelector.Items.Add(
                    new ComboBoxItem
                    {
                        Content = "No Cameras Detected",
                        Tag = string.Empty
                    });

                CameraSelector.SelectedIndex = 0;
                SetDefaultCameraButton.IsEnabled = false;
                StartPreviewButton.IsEnabled = false;
                StopPreviewButton.IsEnabled = false;
                CameraStatusText.Text = "Source: Preview Unavailable";

                CameraStatusText.Text = "Source: No Source Connected";
                Title = "Lab Imager";

                return;
            }

            foreach (var device in devices)
            {
                var isDefault =
                    !string.IsNullOrWhiteSpace(settings.DefaultCameraDevicePath) &&
                    device.DevicePath == settings.DefaultCameraDevicePath;

                var displayName = isDefault
                    ? device.Name + " (Default)"
                    : device.Name;

                CameraSelector.Items.Add(
                    new ComboBoxItem
                    {
                        Content = displayName,
                        Tag = device.DevicePath
                    });
            }

            ComboBoxItem? selectedItem = null;

            if (!string.IsNullOrWhiteSpace(settings.DefaultCameraDevicePath))
            {
                selectedItem = CameraSelector.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item =>
                        item.Tag?.ToString() == settings.DefaultCameraDevicePath);
            }

            if (selectedItem == null && !string.IsNullOrWhiteSpace(settings.DefaultCameraName))
            {
                selectedItem = CameraSelector.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item =>
                        item.Content?.ToString() == settings.DefaultCameraName);
            }

            CameraSelector.SelectedItem = selectedItem ?? CameraSelector.Items[0];

            SetDefaultCameraButton.IsEnabled = true;
            StartPreviewButton.IsEnabled = true;
            FreezePreviewButton.IsEnabled = false;
            FreezePreviewButton.Content = "Freeze";
            StopPreviewButton.IsEnabled = false;
            CameraStatusText.Text = "Source: Preview Idle";

            UpdateSelectedCameraState();
            PopulateCaptureFormatsForSelectedCamera();
        }

        private void CameraSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cameraPreviewService.IsPreviewRunning)
            {
                _cameraPreviewService.StopPreview();
                PreviewHost.Visibility = Visibility.Collapsed;
                ViewportPlaceholder.Visibility = Visibility.Visible;

                CameraStatusText.Text = "Source: Preview Stopped - Source Changed";
                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "Freeze";
                StopPreviewButton.IsEnabled = false;
            }

            UpdateSelectedCameraState();
            PopulateCaptureFormatsForSelectedCamera();
        }

        private void SetDefaultCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (CameraSelector.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            var cameraName = selectedItem.Content?.ToString() ?? string.Empty;
            var cameraDevicePath = selectedItem.Tag?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cameraName) ||
                cameraName == "No Cameras Detected")
            {
                return;
            }

            var cleanCameraName = cameraName.Replace(" (Default)", string.Empty);

            _appSettingsService.Save(
                new Models.Settings.AppSettings
                {
                    DefaultCameraName = cleanCameraName,
                    DefaultCameraDevicePath = cameraDevicePath
                });

            LoadCameraDevices();

            foreach (var item in CameraSelector.Items.OfType<ComboBoxItem>())
            {
                if (string.Equals(item.Tag?.ToString(), cameraDevicePath, StringComparison.OrdinalIgnoreCase))
                {
                    CameraSelector.SelectedItem = item;
                    break;
                }
            }

            CameraStatusText.Text = $"Source: Default Source Set: {cleanCameraName}";
            Title = $"Lab Imager - Default Source: {cleanCameraName}";
        }

        private void StartPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (CameraSelector.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            var sourceName = selectedItem.Content?.ToString() ?? string.Empty;
            var sourceDevicePath = selectedItem.Tag?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(sourceName) ||
                sourceName == "No Cameras Detected")
            {
                CameraStatusText.Text = "Source: Preview Unavailable";
                return;
            }

            var source = new Models.Camera.CameraDeviceInfo
            {
                Name = sourceName,
                DevicePath = sourceDevicePath
            };

            try
            {
                PreviewHost.Visibility = Visibility.Visible;
                ViewportPlaceholder.Visibility = Visibility.Collapsed;
                PreviewHost.UpdateLayout();

                var selectedFormat =
                    CaptureFormatSelector.SelectedItem is ComboBoxItem selectedFormatItem &&
                    selectedFormatItem.Tag is CameraCaptureFormat cameraCaptureFormat
                        ? cameraCaptureFormat
                        : null;

                _cameraPreviewService.StartPreview(
                    source,
                    selectedFormat,
                    PreviewPanel.Handle,
                    PreviewPanel.ClientSize.Width,
                    PreviewPanel.ClientSize.Height);

                CameraStatusText.Text = $"Source: Preview Running: {sourceName}";
                CameraStatusText.Text = $"Source: Preview Started: {sourceName}";

                StartPreviewButton.IsEnabled = false;
                FreezePreviewButton.IsEnabled = true;
                FreezePreviewButton.Content = "Freeze";
                StopPreviewButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                _cameraPreviewService.StopPreview();

                PreviewHost.Visibility = Visibility.Collapsed;
                ViewportPlaceholder.Visibility = Visibility.Visible;

                CameraStatusText.Text = "Source: Preview Failed";
                CameraStatusText.Text = $"Source: Preview Failed: {ex.Message}";

                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "Freeze";
                StopPreviewButton.IsEnabled = false;
            }
        }

        private void FreezePreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_cameraPreviewService.IsPreviewRunning)
            {
                return;
            }

            if (_cameraPreviewService.IsPreviewFrozen)
            {
                _cameraPreviewService.ResumePreview();

                FreezePreviewButton.Content = "Freeze";
                CameraStatusText.Text = "Source: Preview Running";
                CameraStatusText.Text = "Source: Preview Running";
            }
            else
            {
                _cameraPreviewService.FreezePreview();

                FreezePreviewButton.Content = "Freeze";
                CameraStatusText.Text = "Source: Preview Frozen";
                CameraStatusText.Text = "Source: Preview Frozen";
            }
        }
        private void StopPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            _cameraPreviewService.StopPreview();

            PreviewHost.Visibility = Visibility.Collapsed;
            ViewportPlaceholder.Visibility = Visibility.Visible;

            CameraStatusText.Text = "Source: Preview Stopped";
            CameraStatusText.Text = "Source: Preview Stopped";

            StartPreviewButton.IsEnabled = true;
            FreezePreviewButton.IsEnabled = false;
            FreezePreviewButton.Content = "Freeze";
            StopPreviewButton.IsEnabled = false;
        }

        private void RecordPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cameraPreviewService.IsRecording)
                {
                    _cameraPreviewService.StopRecording();
                    _recordingService.Stop();

                    CameraStatusText.Text = "Source: Recording Stopped";
                    return;
                }

                if (!_cameraPreviewService.IsPreviewRunning)
                {
                    CameraStatusText.Text = "Source: Start Preview Before Recording";
                    return;
                }

                var metadata = BuildCurrentSessionMetadata();
                var outputPath = BuildRecordingOutputPath(metadata);

                _cameraPreviewService.StartRecording(outputPath);
                _recordingService.Start();

                CameraStatusText.Text = $"Source: Recording Started: {outputPath}";
            }
            catch (Exception ex)
            {
                CameraStatusText.Text = $"Source: Recording Failed: {ex.Message}";
            }
        }

        private void EnsureRecordingHeaderText()
        {
            if (_recordingHeaderText is not null)
            {
                return;
            }

            _recordingHeaderText = new TextBlock
            {
                Text = "REC 00:00:00",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(0, -3, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0xFF, 0x30, 0x30)),
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false
            };

            System.Windows.Controls.Panel.SetZIndex(_recordingHeaderText, 100);

            if (ViewportSurface.Parent is System.Windows.Controls.Panel panel)
            {
                panel.Children.Add(_recordingHeaderText);
            }
        }

        private void UpdateRecordingUi()
        {
            EnsureRecordingHeaderText();

            var elapsed = _recordingService.Elapsed.ToString(@"hh\:mm\:ss");

            RecordingStateText.Text = $"Recording: {_recordingService.State}";
            RecordingTimeText.Text = elapsed;

            _recordingHeaderText!.Text = $"REC {elapsed}";
            _recordingHeaderText.Visibility =
                _recordingService.State == RecordingState.Recording
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private static string BuildRecordingOutputPath(SessionMetadata metadata)
        {
            var outputDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "Lab Imager",
                "Recordings");

            Directory.CreateDirectory(outputDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            var parts = new[]
                {
                    SanitizeFileNamePart(metadata.ProjectName),
                    SanitizeFileNamePart(metadata.BoardOrDevice),
                    SanitizeFileNamePart(metadata.Technician)
                }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToList();

            var baseName = parts.Count > 0
                ? timestamp + "_" + string.Join("_", parts)
                : timestamp + "_lab-imager-recording";

            return Path.Combine(outputDir, baseName + ".avi");
        }

        private static string SanitizeFileNamePart(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(
                value
                    .Trim()
                    .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
                    .ToArray());

            while (cleaned.Contains("__", StringComparison.Ordinal))
            {
                cleaned = cleaned.Replace("__", "_");
            }

            return cleaned.Trim('_', ' ');
        }
        private void CaptureScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var metadata = BuildCurrentSessionMetadata();

                var originalPath = CapturePreviewPanelToPng();
                var evidencePath = _viewportCaptureService.CreateEvidencePng(originalPath, metadata);

                CameraStatusText.Text = $"Source: Evidence Capture Saved: {evidencePath}";
            }
            catch (Exception ex)
            {
                CameraStatusText.Text = $"Source: Screenshot Failed: {ex.Message}";
            }
        }

        private SessionMetadata BuildCurrentSessionMetadata()
        {
            var selectedCamera = CameraSelector.SelectedItem as ComboBoxItem;

            return new SessionMetadata
            {
                ProjectName = ProjectNameTextBox.Text?.Trim() ?? string.Empty,
                BoardOrDevice = BoardDeviceTextBox.Text?.Trim() ?? string.Empty,
                Technician = TechnicianTextBox.Text?.Trim() ?? string.Empty,
                SourceName = selectedCamera?.Content?.ToString()?.Trim() ?? string.Empty,
                SourceDevicePath = selectedCamera?.Tag?.ToString()?.Trim() ?? string.Empty,
                NotesText = GetNotesPlainText()
            };
        }

        private string GetNotesPlainText()
        {
            var range = new TextRange(NotesEditor.Document.ContentStart, NotesEditor.Document.ContentEnd);
            return range.Text.Trim();
        }

        private string CapturePreviewPanelToPng()
        {
            if (PreviewPanel.ClientSize.Width <= 0 || PreviewPanel.ClientSize.Height <= 0)
            {
                throw new InvalidOperationException("Preview panel has no rendered size to capture.");
            }

            var capturesDirectory = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "Lab Imager",
                "Captures");

            System.IO.Directory.CreateDirectory(capturesDirectory);

            var fileName = $"lab-imager-capture-{DateTime.Now:yyyyMMdd-HHmmss}-original.png";
            var filePath = System.IO.Path.Combine(capturesDirectory, fileName);

            var screenPoint = PreviewPanel.PointToScreen(System.Drawing.Point.Empty);

            using var bitmap = new System.Drawing.Bitmap(
                PreviewPanel.ClientSize.Width,
                PreviewPanel.ClientSize.Height);

            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    screenPoint.X,
                    screenPoint.Y,
                    0,
                    0,
                    PreviewPanel.ClientSize);
            }

            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            return filePath;
        }
        private void PopulateCaptureFormatsForSelectedCamera()
        {
            CaptureFormatSelector.Items.Clear();

            if (CameraSelector.SelectedItem is not ComboBoxItem selectedItem)
            {
                CaptureFormatSelector.IsEnabled = false;
                return;
            }

            var cameraName = selectedItem.Content?.ToString()?.Replace(" (Default)", string.Empty) ?? string.Empty;
            var cameraDevicePath = selectedItem.Tag?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cameraName) ||
                cameraName == "No Cameras Detected")
            {
                CaptureFormatSelector.IsEnabled = false;
                return;
            }

            var device = new Models.Camera.CameraDeviceInfo
            {
                Name = cameraName,
                DevicePath = cameraDevicePath
            };

            var formats = _cameraDeviceService.GetAvailableFormats(device);

            foreach (var format in formats)
            {
                CaptureFormatSelector.Items.Add(new ComboBoxItem
                {
                    Content = format.DisplayName,
                    Tag = format,
                    IsEnabled = format.IsAvailable
                });
            }

            CaptureFormatSelector.IsEnabled = CaptureFormatSelector.Items.Count > 0;

            if (CaptureFormatSelector.Items.Count > 0)
            {
                CaptureFormatSelector.SelectedIndex = 0;
            }
        }
        private void UpdateSelectedCameraState()
        {
            if (CameraSelector.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            var cameraName = selectedItem.Content?.ToString();

            if (string.IsNullOrWhiteSpace(cameraName) ||
                cameraName == "No Cameras Detected")
            {
                CameraStatusText.Text = "Source: No Source Connected";
                Title = "Lab Imager";
                SetDefaultCameraButton.IsEnabled = false;
                StartPreviewButton.IsEnabled = false;
                StopPreviewButton.IsEnabled = false;
                CameraStatusText.Text = "Source: Preview Unavailable";

                return;
            }

            CameraStatusText.Text = $"Source: {cameraName}";
            Title = $"Lab Imager - {cameraName}";
            SetDefaultCameraButton.IsEnabled = true;

            if (!_cameraPreviewService.IsPreviewRunning)
            {
                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "Freeze";
                StopPreviewButton.IsEnabled = false;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximizeRestore();
                return;
            }

            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximizeRestore();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NotesFontSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesEditor is null ||
                NotesFontSelector.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            var fontName = selectedItem.Content?.ToString();

            if (string.IsNullOrWhiteSpace(fontName))
            {
                return;
            }

            var fontFamily = new System.Windows.Media.FontFamily(fontName);

            if (!NotesEditor.Selection.IsEmpty)
            {
                NotesEditor.Selection.ApplyPropertyValue(
                    TextElement.FontFamilyProperty,
                    fontFamily
                );
            }
            else
            {
                NotesEditor.FontFamily = fontFamily;
            }

            NotesEditor.Focus();
        }

        private void NotesBoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesUnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesUndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesEditor.CanUndo)
            {
                NotesEditor.Undo();
            }

            NotesEditor.Focus();
        }

        private void NotesRedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesEditor.CanRedo)
            {
                NotesEditor.Redo();
            }

            NotesEditor.Focus();
        }

        private void NotesBulletButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBullets.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesOutlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleNumbering.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesIndentButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.IncreaseIndentation.Execute(null, NotesEditor);
            NotesEditor.Focus();
        }

        private void NotesAlignButton_Click(object sender, RoutedEventArgs e)
        {
            var paragraph = NotesEditor.CaretPosition.Paragraph;

            if (paragraph is null)
            {
                return;
            }

            var nextAlignment = paragraph.TextAlignment switch
            {
                TextAlignment.Left => TextAlignment.Center,
                TextAlignment.Center => TextAlignment.Right,
                TextAlignment.Right => TextAlignment.Justify,
                _ => TextAlignment.Left
            };

            NotesEditor.Selection.ApplyPropertyValue(
                Block.TextAlignmentProperty,
                nextAlignment
            );

            paragraph.TextAlignment = nextAlignment;
            NotesEditor.Focus();
        }

        private void NotesColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesColorButton.ContextMenu is null)
            {
                return;
            }

            NotesColorButton.ContextMenu.PlacementTarget = NotesColorButton;
            NotesColorButton.ContextMenu.IsOpen = true;
        }

        private void NotesTextColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem ||
                menuItem.Tag is not string colorName)
            {
                return;
            }

            var brush = new System.Windows.Media.BrushConverter().ConvertFromString(colorName)
                        as System.Windows.Media.Brush;

            if (brush is null)
            {
                return;
            }

            NotesEditor.Selection.ApplyPropertyValue(
                TextElement.ForegroundProperty,
                brush
            );

            NotesEditor.Focus();
        }

        private void ToggleMaximizeRestore()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}


