using Weak.Models;

namespace Weak.Services;

public class SettingsService
{
    private readonly DatabaseService _database;
    private UserSettings? _cachedSettings;

    public SettingsService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<UserSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        _cachedSettings = await _database.GetUserSettingsAsync();
        return _cachedSettings;
    }

    public async Task SaveSettingsAsync(UserSettings settings)
    {
        await _database.SaveUserSettingsAsync(settings);
        _cachedSettings = settings;
    }

    public async Task<bool> IsOnboardingCompleteAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.OnboardingComplete;
    }

    public async Task MarkOnboardingCompleteAsync()
    {
        var settings = await GetSettingsAsync();
        settings.OnboardingComplete = true;
        await SaveSettingsAsync(settings);
    }
}
