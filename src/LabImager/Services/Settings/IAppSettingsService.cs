using LabImager.Models.Settings;

namespace LabImager.Services.Settings;

public interface IAppSettingsService
{
AppSettings Load();

void Save(AppSettings settings);

}
