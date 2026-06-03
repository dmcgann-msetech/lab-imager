using System.Windows;
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

        _cameraDeviceService = new StubCameraDeviceService();
        _appSettingsService = new StubAppSettingsService();

        var settings = _appSettingsService.Load();
        var devices = _cameraDeviceService.GetAvailableDevices();

        Title = !string.IsNullOrWhiteSpace(settings.DefaultCameraName)
            ? $"Lab Imager - Default Camera: {settings.DefaultCameraName}"
            : devices.Count > 0
                ? $"Lab Imager - {devices[0].Name}"
                : "Lab Imager";
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
