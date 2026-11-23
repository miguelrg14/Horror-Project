using System;

namespace Horror.Core
{
    /// <summary>Key-value storage behind the settings (PlayerPrefs in the game, a dictionary in tests).</summary>
    public interface ISettingsStorage
    {
        bool HasKey(string key);
        float GetFloat(string key, float defaultValue);
        void SetFloat(string key, float value);
        int GetInt(string key, int defaultValue);
        void SetInt(string key, int value);
        void Save();
    }

    public enum VolumeChannel
    {
        Master,
        Music,
        Effects,
        Interface,
        Voice
    }

    /// <summary>
    /// Player options with their keys, defaults and valid ranges, shared by the main menu and the pause menu.
    /// </summary>
    public sealed class GameSettings
    {
        public const float DefaultVolume = 1f;
        public const float DefaultBrightness = 1f;
        public const float MaxOverlayAlpha = 0.8f;
        public const float DefaultSensitivity = 5f;
        public const float MinSensitivity = 0.5f;
        public const float MaxSensitivity = 8f;

        public const string QualityKey = "quality";
        public const string ResolutionKey = "resolution";
        public const string FullscreenKey = "fullscreen";
        public const string BrightnessKey = "brightnessLevel";
        public const string SensitivityKey = "sensitivity";
        // Version 2 stores volumes as 0-1 slider values; version 1 stored raw decibels.
        private const string VolumeVersionKey = "volumeFormat";
        private const int VolumeVersion = 2;

        private readonly ISettingsStorage storage;

        public GameSettings(ISettingsStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            if (storage.GetInt(VolumeVersionKey, 0) != VolumeVersion)
            {
                foreach (VolumeChannel channel in Enum.GetValues(typeof(VolumeChannel)))
                {
                    storage.SetFloat(VolumeKey(channel), DefaultVolume);
                }
                storage.SetInt(VolumeVersionKey, VolumeVersion);
            }
        }

        /// <summary>Exposed AudioMixer parameter (and storage key) for each channel.</summary>
        public static string VolumeKey(VolumeChannel channel)
        {
            switch (channel)
            {
                case VolumeChannel.Master: return "masterVolume";
                case VolumeChannel.Music: return "musicVolume";
                case VolumeChannel.Effects: return "sfxVolume";
                case VolumeChannel.Interface: return "uiVolume";
                case VolumeChannel.Voice: return "voiceVolume";
                default: throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        public float GetVolume(VolumeChannel channel) => Clamp(storage.GetFloat(VolumeKey(channel), DefaultVolume), 0f, 1f);

        public float SetVolume(VolumeChannel channel, float linear)
        {
            float value = Clamp(linear, 0f, 1f);
            storage.SetFloat(VolumeKey(channel), value);
            return value;
        }

        /// <summary>0 = darkest, 1 = no dimming.</summary>
        public float Brightness
        {
            get => Clamp(storage.GetFloat(BrightnessKey, DefaultBrightness), 0f, 1f);
            set => storage.SetFloat(BrightnessKey, Clamp(value, 0f, 1f));
        }

        /// <summary>Alpha of the black full-screen overlay that dims the picture.</summary>
        public static float OverlayAlpha(float brightness) => (1f - Clamp(brightness, 0f, 1f)) * MaxOverlayAlpha;

        public float Sensitivity
        {
            get => Clamp(storage.GetFloat(SensitivityKey, DefaultSensitivity), MinSensitivity, MaxSensitivity);
            set => storage.SetFloat(SensitivityKey, Clamp(value, MinSensitivity, MaxSensitivity));
        }

        /// <summary>Multiplier applied to the mouse input (1 at the default sensitivity).</summary>
        public float SensitivityMultiplier => Sensitivity / DefaultSensitivity;

        public int GetQuality(int defaultLevel) => storage.GetInt(QualityKey, defaultLevel);

        public void SetQuality(int level) => storage.SetInt(QualityKey, Math.Max(0, level));

        public bool GetFullscreen(bool defaultValue) => storage.GetInt(FullscreenKey, defaultValue ? 1 : 0) == 1;

        public void SetFullscreen(bool value) => storage.SetInt(FullscreenKey, value ? 1 : 0);

        public int GetResolutionIndex(int defaultIndex) => storage.GetInt(ResolutionKey, defaultIndex);

        public void SetResolutionIndex(int index) => storage.SetInt(ResolutionKey, Math.Max(0, index));

        public void Save() => storage.Save();

        private static float Clamp(float value, float min, float max)
        {
            if (float.IsNaN(value)) return min;
            return value < min ? min : value > max ? max : value;
        }
    }
}
