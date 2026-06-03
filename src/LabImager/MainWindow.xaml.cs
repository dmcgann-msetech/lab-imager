using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LabImager.Services.Camera;
using LabImager.Services.Settings;

namespace LabImager
{
    public partial class MainWindow : Window
    {
        private readonly ICameraDeviceService _cameraDeviceService;
        private readonly IAppSettingsService _appSettingsService;

        public MainWindow()
        {
            InitializeComponent();

            _cameraDeviceService = new DirectShowCameraDeviceService();
            _appSettingsService = new StubAppSettingsService();

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

                CameraStatusText.Text = "📷  No Camera Connected";
                Title = "Lab Imager";

                return;
            }

            foreach (var device in devices)
            {
                CameraSelector.Items.Add(
                    new ComboBoxItem
                    {
                        Content = device.Name,
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

            UpdateSelectedCameraState();
        }

        private void CameraSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

            CameraStatusText.Text = $"📷  Default Camera Set: {cameraName}";
            Title = $"Lab Imager - Default Camera: {cameraName}";
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
                CameraStatusText.Text = "📷  No Camera Connected";
                Title = "Lab Imager";
                SetDefaultCameraButton.IsEnabled = false;

                return;
            }

            CameraStatusText.Text = $"📷  {cameraName}";
            Title = $"Lab Imager - {cameraName}";
            SetDefaultCameraButton.IsEnabled = true;
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
