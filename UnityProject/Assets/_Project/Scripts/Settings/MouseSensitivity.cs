using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Applies the sensitivity chosen in the options to the mouse input read by <see cref="InputPlayer"/>.
/// It runs after InputPlayer has read the axes and before FPSCamera uses them (see the script
/// execution order in their .meta files), so the camera scripts did not need to change.
/// </summary>
[DefaultExecutionOrder(-50)]
public class MouseSensitivity : MonoBehaviour
{
    private InputPlayer input;
    private bool searched;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        var host = new GameObject(nameof(MouseSensitivity));
        host.hideFlags = HideFlags.HideInHierarchy;
        DontDestroyOnLoad(host);
        host.AddComponent<MouseSensitivity>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => searched = false;

    private void Update()
    {
        if (!searched)
        {
            input = FindObjectOfType<InputPlayer>();
            searched = true;
        }
        if (input == null) return;

        float multiplier = SettingsService.Current.SensitivityMultiplier;
        input.mouseX *= multiplier;
        input.mouseY *= multiplier;
    }
}
