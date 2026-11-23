using System.Collections.Generic;
using System.Linq;
using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Display and control options of the options panel (shared by the main menu and the pause menu):
/// resolution, quality, fullscreen, brightness and mouse sensitivity.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown qualityDropdown;

    [Header("Brightness")]
    [SerializeField] private Slider brightnessSlider;
    [Tooltip("Black full-screen image whose alpha dims the picture.")]
    [SerializeField, FormerlySerializedAs("brightnesspanel")] private Image brightnessOverlay;

    [Header("Mouse")]
    [SerializeField, FormerlySerializedAs("sensivitySlider")] private Slider sensitivitySlider;

    private List<ScreenSize> resolutions = new List<ScreenSize>();

    private void Start()
    {
        GameSettings settings = SettingsService.Current;
        BuildResolutionDropdown(settings);

        if (qualityDropdown != null)
        {
            int quality = Mathf.Clamp(settings.GetQuality(QualitySettings.GetQualityLevel()), 0, QualitySettings.names.Length - 1);
            qualityDropdown.SetValueWithoutNotify(quality);
            QualitySettings.SetQualityLevel(quality);
        }

        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;
            brightnessSlider.SetValueWithoutNotify(settings.Brightness);
        }
        ApplyBrightness(settings.Brightness);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = GameSettings.MinSensitivity;
            sensitivitySlider.maxValue = GameSettings.MaxSensitivity;
            sensitivitySlider.SetValueWithoutNotify(settings.Sensitivity);
        }
    }

    private void BuildResolutionDropdown(GameSettings settings)
    {
        if (resolutionDropdown == null) return;

        resolutions = ResolutionOptions.Distinct(Screen.resolutions.Select(r => new ScreenSize(r.width, r.height)));
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(ResolutionOptions.Labels(resolutions));

        // The options must exist before the current value is selected.
        int current = ResolutionOptions.IndexOf(resolutions, Screen.width, Screen.height);
        resolutionDropdown.SetValueWithoutNotify(Mathf.Max(0, current));
        resolutionDropdown.RefreshShownValue();
        // Changing the resolution is meaningless inside a browser page.
        resolutionDropdown.interactable = Application.platform != RuntimePlatform.WebGLPlayer && resolutions.Count > 1;
    }

    // UI callbacks (names kept from the original options prefab).
    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Count) return;
        ScreenSize size = resolutions[index];
        Screen.SetResolution(size.Width, size.Height, Screen.fullScreen);
        SettingsService.Current.SetResolutionIndex(index);
    }

    public void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(level);
        SettingsService.Current.SetQuality(level);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SettingsService.Current.SetFullscreen(isFullscreen);
    }

    public void ChangeBrightness(float value)
    {
        SettingsService.Current.Brightness = value;
        ApplyBrightness(SettingsService.Current.Brightness);
    }

    public void ChangeSensivityMainMenu(float value) => SettingsService.Current.Sensitivity = value;

    private void OnDisable() => SettingsService.Current.Save();

    private void ApplyBrightness(float brightness)
    {
        if (brightnessOverlay == null) return;
        Color color = brightnessOverlay.color;
        color.a = GameSettings.OverlayAlpha(brightness);
        brightnessOverlay.color = color;
    }
}
