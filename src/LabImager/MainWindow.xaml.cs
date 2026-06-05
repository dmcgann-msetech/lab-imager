using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using LabImager.Services.Camera;
using LabImager.Services.Capture;
using LabImager.Services.Preview;
using LabImager.Services.Settings;

namespace LabImager
{
    public partial class MainWindow : Window
    {
        private readonly ICameraDeviceService _cameraDeviceService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IViewportCaptureService _viewportCaptureService;
        private readonly ICameraPreviewService _cameraPreviewService;

        public MainWindow()
        {
            InitializeComponent();

            _cameraDeviceService = new DirectShowCameraDeviceService();
            _appSettingsService = new JsonAppSettingsService();
            _viewportCaptureService = new ViewportCaptureService();
            _cameraPreviewService = new DirectShowCameraPreviewService();

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
                PreviewStatusText.Text = "Preview Unavailable";

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
                    ? "★ " + device.Name + " (Default)"
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
            FreezePreviewButton.Content = "❄";
            StopPreviewButton.IsEnabled = false;
            PreviewStatusText.Text = "Preview Idle";

            UpdateSelectedCameraState();
        }

        private void CameraSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cameraPreviewService.IsPreviewRunning)
            {
                _cameraPreviewService.StopPreview();
                PreviewHost.Visibility = Visibility.Collapsed;
                ViewportPlaceholder.Visibility = Visibility.Visible;

                PreviewStatusText.Text = "Preview Stopped - Source Changed";
                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "❄";
                StopPreviewButton.IsEnabled = false;
            }

            UpdateSelectedCameraState();
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

            var cleanCameraName = cameraName.Replace("★ ", string.Empty).Replace(" (Default)", string.Empty);

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
                PreviewStatusText.Text = "Preview Unavailable";
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

                _cameraPreviewService.StartPreview(
                    source,
                    PreviewPanel.Handle,
                    PreviewPanel.ClientSize.Width,
                    PreviewPanel.ClientSize.Height);

                PreviewStatusText.Text = $"Preview Running: {sourceName}";
                CameraStatusText.Text = $"Source: Preview Started: {sourceName}";

                StartPreviewButton.IsEnabled = false;
                FreezePreviewButton.IsEnabled = true;
                FreezePreviewButton.Content = "❄";
                StopPreviewButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                _cameraPreviewService.StopPreview();

                PreviewHost.Visibility = Visibility.Collapsed;
                ViewportPlaceholder.Visibility = Visibility.Visible;

                PreviewStatusText.Text = "Preview Failed";
                CameraStatusText.Text = $"Source: Preview Failed: {ex.Message}";

                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "❄";
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

                FreezePreviewButton.Content = "❄";
                PreviewStatusText.Text = "Preview Running";
                CameraStatusText.Text = "Source: Preview Running";
            }
            else
            {
                _cameraPreviewService.FreezePreview();

                FreezePreviewButton.Content = "▶";
                PreviewStatusText.Text = "Preview Frozen";
                CameraStatusText.Text = "Source: Preview Frozen";
            }
        }
        private void StopPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            _cameraPreviewService.StopPreview();

            PreviewHost.Visibility = Visibility.Collapsed;
            ViewportPlaceholder.Visibility = Visibility.Visible;

            PreviewStatusText.Text = "Preview Stopped";
            CameraStatusText.Text = "Source: Preview Stopped";

            StartPreviewButton.IsEnabled = true;
            FreezePreviewButton.IsEnabled = false;
            FreezePreviewButton.Content = "❄";
            StopPreviewButton.IsEnabled = false;
        }

        private void CaptureScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var savedPath = _viewportCaptureService.CaptureElementToPng(ViewportSurface);
                CameraStatusText.Text = $"Source: Screenshot Saved: {savedPath}";
            }
            catch (Exception ex)
            {
                CameraStatusText.Text = $"Source: Screenshot Failed: {ex.Message}";
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
                PreviewStatusText.Text = "Preview Unavailable";

                return;
            }

            CameraStatusText.Text = $"Source: {cameraName}";
            Title = $"Lab Imager - {cameraName}";
            SetDefaultCameraButton.IsEnabled = true;

            if (!_cameraPreviewService.IsPreviewRunning)
            {
                StartPreviewButton.IsEnabled = true;
                FreezePreviewButton.IsEnabled = false;
                FreezePreviewButton.Content = "❄";
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



















