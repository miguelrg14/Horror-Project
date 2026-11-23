using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Volume sliders of the options panel. Each slider goes from 0 to 1 and is converted to decibels
/// for its exposed AudioMixer parameter; the values are saved between sessions.
/// </summary>
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders (0-1)")]
    [SerializeField, FormerlySerializedAs("GeneralVolumeSlider")] private Slider masterSlider;
    [SerializeField, FormerlySerializedAs("MusicVolumeSlider")] private Slider musicSlider;
    [SerializeField, FormerlySerializedAs("EffectsVolumeSlider")] private Slider effectsSlider;
    [SerializeField, FormerlySerializedAs("UIVolumeSlider")] private Slider interfaceSlider;
    [SerializeField, FormerlySerializedAs("VoiceVolumeSlider")] private Slider voiceSlider;

    private void Start()
    {
        // AudioMixer.SetFloat is ignored during Awake, so the saved volumes are applied here.
        Load(VolumeChannel.Master, masterSlider);
        Load(VolumeChannel.Music, musicSlider);
        Load(VolumeChannel.Effects, effectsSlider);
        Load(VolumeChannel.Interface, interfaceSlider);
        Load(VolumeChannel.Voice, voiceSlider);
    }

    // Slider OnValueChanged callbacks (names kept from the original options prefab).
    public void SetVolume(float value) => Apply(VolumeChannel.Master, value, true);

    public void SetMusic(float value) => Apply(VolumeChannel.Music, value, true);

    public void SetSFX(float value) => Apply(VolumeChannel.Effects, value, true);

    public void SetUI(float value) => Apply(VolumeChannel.Interface, value, true);

    public void SetVoice(float value) => Apply(VolumeChannel.Voice, value, true);

    private void Load(VolumeChannel channel, Slider slider)
    {
        float value = SettingsService.Current.GetVolume(channel);
        if (slider != null) slider.SetValueWithoutNotify(value);
        Apply(channel, value, false);
    }

    private void Apply(VolumeChannel channel, float value, bool save)
    {
        if (save) value = SettingsService.Current.SetVolume(channel, value);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(GameSettings.VolumeKey(channel), VolumeMath.LinearToDecibels(value));
        }
    }
}
