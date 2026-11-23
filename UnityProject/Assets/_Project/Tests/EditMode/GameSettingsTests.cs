using System.Collections.Generic;
using HorrorProject.Core;
using NUnit.Framework;

namespace HorrorProject.Tests
{
    public class GameSettingsTests
    {
        private sealed class MemoryStorage : ISettingsStorage
        {
            public readonly Dictionary<string, float> Floats = new Dictionary<string, float>();
            public readonly Dictionary<string, int> Ints = new Dictionary<string, int>();
            public int Saves;

            public bool HasKey(string key) => Floats.ContainsKey(key) || Ints.ContainsKey(key);
            public float GetFloat(string key, float defaultValue) => Floats.TryGetValue(key, out float v) ? v : defaultValue;
            public void SetFloat(string key, float value) => Floats[key] = value;
            public int GetInt(string key, int defaultValue) => Ints.TryGetValue(key, out int v) ? v : defaultValue;
            public void SetInt(string key, int value) => Ints[key] = value;
            public void Save() => Saves++;
        }

        [Test]
        public void FreshInstall_StartsAtFullVolumeAndFullBrightness()
        {
            var settings = new GameSettings(new MemoryStorage());

            Assert.AreEqual(1f, settings.GetVolume(VolumeChannel.Master));
            Assert.AreEqual(1f, settings.GetVolume(VolumeChannel.Voice));
            Assert.AreEqual(1f, settings.Brightness);
            Assert.AreEqual(0f, GameSettings.OverlayAlpha(settings.Brightness));
            Assert.AreEqual(1f, settings.SensitivityMultiplier);
        }

        [Test]
        public void OldDecibelVolumes_AreReplacedByDefaults()
        {
            var storage = new MemoryStorage();
            storage.Floats["musicVolume"] = -35f; // saved by the original -80..0 dB sliders

            var settings = new GameSettings(storage);

            Assert.AreEqual(1f, settings.GetVolume(VolumeChannel.Music));
        }

        [Test]
        public void Volumes_AreClampedAndPersisted()
        {
            var storage = new MemoryStorage();
            var settings = new GameSettings(storage);

            Assert.AreEqual(0.4f, settings.SetVolume(VolumeChannel.Effects, 0.4f));
            Assert.AreEqual(1f, settings.SetVolume(VolumeChannel.Interface, 7f));
            Assert.AreEqual(0f, settings.SetVolume(VolumeChannel.Voice, -2f));

            var reloaded = new GameSettings(storage);
            Assert.AreEqual(0.4f, reloaded.GetVolume(VolumeChannel.Effects));
            Assert.AreEqual(1f, reloaded.GetVolume(VolumeChannel.Interface));
            Assert.AreEqual(0f, reloaded.GetVolume(VolumeChannel.Voice));
        }

        [Test]
        public void EachChannel_HasTheMixerParameterName()
        {
            Assert.AreEqual("masterVolume", GameSettings.VolumeKey(VolumeChannel.Master));
            Assert.AreEqual("musicVolume", GameSettings.VolumeKey(VolumeChannel.Music));
            Assert.AreEqual("sfxVolume", GameSettings.VolumeKey(VolumeChannel.Effects));
            Assert.AreEqual("uiVolume", GameSettings.VolumeKey(VolumeChannel.Interface));
            Assert.AreEqual("voiceVolume", GameSettings.VolumeKey(VolumeChannel.Voice));
        }

        [TestCase(1f, 0f)]
        [TestCase(0.5f, 0.4f)]
        [TestCase(0f, 0.8f)]
        public void LowerBrightness_DarkensTheOverlay(float brightness, float alpha)
        {
            Assert.AreEqual(alpha, GameSettings.OverlayAlpha(brightness), 0.0001f);
        }

        [Test]
        public void Sensitivity_IsClampedToTheSliderRange()
        {
            var settings = new GameSettings(new MemoryStorage());

            settings.Sensitivity = 100f;
            Assert.AreEqual(GameSettings.MaxSensitivity, settings.Sensitivity);
            settings.Sensitivity = 0f;
            Assert.AreEqual(GameSettings.MinSensitivity, settings.Sensitivity);
            settings.Sensitivity = 10f / 2f;
            Assert.AreEqual(1f, settings.SensitivityMultiplier);
        }

        [Test]
        public void QualityFullscreenAndResolution_RoundTrip()
        {
            var storage = new MemoryStorage();
            var settings = new GameSettings(storage);

            Assert.AreEqual(3, settings.GetQuality(3));
            settings.SetQuality(1);
            settings.SetFullscreen(false);
            settings.SetResolutionIndex(-4);
            settings.Save();

            Assert.AreEqual(1, settings.GetQuality(3));
            Assert.IsFalse(settings.GetFullscreen(true));
            Assert.AreEqual(0, settings.GetResolutionIndex(5));
            Assert.AreEqual(1, storage.Saves);
        }
    }
}
