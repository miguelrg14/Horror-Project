using HorrorProject.Core;
using UnityEngine;

/// <summary>Stores the game settings in PlayerPrefs.</summary>
public sealed class PlayerPrefsStorage : ISettingsStorage
{
    public bool HasKey(string key) => PlayerPrefs.HasKey(key);

    public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);

    public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);

    public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);

    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);

    public void Save() => PlayerPrefs.Save();
}
