using Horror.Core;

/// <summary>Single shared settings object for the whole game.</summary>
public static class SettingsService
{
    private static GameSettings current;

    public static GameSettings Current => current ?? (current = new GameSettings(new PlayerPrefsStorage()));
}
