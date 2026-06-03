using LabImager.Models.Settings;

namespace LabImager.Services.Settings;

public sealed class StubAppSettingsService : IAppSettingsService
{
private readonly AppSettings _settings = new();

public AppSettings Load()
{
    return _settings;
}

public void Save(AppSettings settings)
{
    _settings.DefaultCameraDevicePath = settings.DefaultCameraDevicePath;
    _settings.DefaultCameraName = settings.DefaultCameraName;
}

}
