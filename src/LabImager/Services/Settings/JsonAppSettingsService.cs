using System;
using System.IO;
using System.Text.Json;
using LabImager.Models.Settings;

namespace LabImager.Services.Settings;

public sealed class JsonAppSettingsService : IAppSettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsFilePath;

    public JsonAppSettingsService()
    {
        var settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Lab Imager");

        Directory.CreateDirectory(settingsDirectory);

        _settingsFilePath = Path.Combine(settingsDirectory, "appsettings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(_settingsFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new AppSettings();
        }

        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, SerializerOptions);
        File.WriteAllText(_settingsFilePath, json);
    }
}
