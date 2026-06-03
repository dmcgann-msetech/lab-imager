using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            _cameraPreviewService = new StubCameraPreviewService();

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
            StopPreviewButton.IsEnabled = false;
            PreviewStatusText.Text = "Preview Idle";

            UpdateSelectedCameraState();
        }

        private void CameraSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cameraPreviewService.IsPreviewRunning)
            {
                _cameraPreviewService.StopPreview();
                PreviewStatusText.Text = "Preview Stopped - Source Changed";
                StartPreviewButton.IsEnabled = true;
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

            _appSettingsService.Save(
                new Models.Settings.AppSettings
                {
                    DefaultCameraName = cameraName,
                    DefaultCameraDevicePath = cameraDevicePath
                });

            CameraStatusText.Text = $"Source: Default Source Set: {cameraName}";
            Title = $"Lab Imager - Default Source: {cameraName}";
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

            _cameraPreviewService.StartPreview(source);

            PreviewStatusText.Text = $"Preview Running: {sourceName}";
            CameraStatusText.Text = $"Source: Preview Started: {sourceName}";

            StartPreviewButton.IsEnabled = false;
            StopPreviewButton.IsEnabled = true;
        }

        private void StopPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            _cameraPreviewService.StopPreview();

            PreviewStatusText.Text = "Preview Stopped";
            CameraStatusText.Text = "Source: Preview Stopped";

            StartPreviewButton.IsEnabled = true;
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

        private void ToggleMaximizeRestore()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}


