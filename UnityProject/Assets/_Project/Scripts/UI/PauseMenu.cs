using Horror.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// In-game pause menu. Escape pauses and resumes the game; when the options panel is open,
/// Escape goes back to the pause menu instead. While paused, time is stopped, the cursor is freed,
/// the camera stops reading the mouse and a heartbeat loops in the background.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public const string MainMenuScene = "Menu";

    /// <summary>True while the game is paused (pause menu or options panel open).</summary>
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject pauseMenuUI;
    [Tooltip("Options panel opened from the pause menu (optional).")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField, FormerlySerializedAs("Player")] private Transform player;

    private readonly PauseStateMachine state = new PauseStateMachine();
    private AudioSource heartbeat;

    public PauseState State => state.State;

    private void Awake()
    {
        IsPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
        else if (state.State == PauseState.Options && optionsPanel != null && !optionsPanel.activeSelf)
        {
            // The options panel was closed with its own "Apply" button: show the pause menu again.
            ApplyButton();
        }
    }

    public void HandleEscape()
    {
        switch (state.State)
        {
            case PauseState.Playing: Pause(); break;
            case PauseState.Paused: Resume(); break;
            default: ApplyButton(); break;
        }
    }

    public void Pause()
    {
        state.Pause();
        SetPaused(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        SetMenuVisible(true);

        PlayerUIManager.instance?.PlayUI("enterMenu");
        if (heartbeat == null && PlayerSFXManager.instance != null)
        {
            heartbeat = PlayerSFXManager.instance.PlaySFX("heartBeat2");
            if (heartbeat != null) heartbeat.loop = true;
        }
    }

    public void Resume()
    {
        state.Resume();
        SetPaused(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        SetMenuVisible(false);

        PlayerUIManager.instance?.PlayUI("enterMenu");
        StopHeartbeat();
    }

    // Called by the "Options" button (which also shows the options panel).
    public void LoadOptions() => state.OpenOptions();

    // Called when the options panel is closed.
    public void ApplyButton()
    {
        if (state.State != PauseState.Options) return;
        state.CloseOptions();
        if (optionsPanel != null) optionsPanel.SetActive(false);
        SetMenuVisible(true);
    }

    public void LoadMenu()
    {
        StopHeartbeat();
        state.Resume();
        SetPaused(false);
        SceneManager.LoadScene(MainMenuScene);
    }

    public void QuitMenu()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // Leaving the scene while paused must not leave the next scene frozen.
        if (state.IsPaused) Time.timeScale = 1f;
        IsPaused = false;
    }

    private void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        if (player != null)
        {
            FPSCamera fpsCamera = player.GetComponent<FPSCamera>();
            if (fpsCamera != null) fpsCamera.enabled = !paused;
        }
    }

    private void SetMenuVisible(bool visible)
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(visible);
    }

    private void StopHeartbeat()
    {
        PlayerSFXManager.Stop(heartbeat);
        heartbeat = null;
    }
}
