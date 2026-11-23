using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Automated playthrough used by CI to check that a real build runs. Start the player with
/// <c>-smoke-test &lt;output folder&gt;</c>: it opens the main menu and its options, enters every level,
/// pauses and resumes, shows the credits, saves a screenshot of each step and writes
/// <c>smoke-results.txt</c>. The process exits with code 1 if any error or exception was logged.
/// Without the argument this component is never created.
/// </summary>
public class SmokeTestRunner : MonoBehaviour
{
    private const string Argument = "-smoke-test";
    private static readonly string[] Levels = { "Level1", "Level2", "Level3" };

    private readonly List<string> errors = new List<string>();
    private readonly StringBuilder report = new StringBuilder();
    private string outputFolder;
    private int screenshotIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        string folder = ReadOutputFolder(Environment.GetCommandLineArgs());
        if (folder == null) return;

        var host = new GameObject(nameof(SmokeTestRunner));
        DontDestroyOnLoad(host);
        host.AddComponent<SmokeTestRunner>().outputFolder = folder;
    }

    public static string ReadOutputFolder(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != Argument) continue;
            return i + 1 < args.Length && !args[i + 1].StartsWith("-") ? args[i + 1] : "smoke-output";
        }
        return null;
    }

    private void Awake()
    {
        Application.logMessageReceived += OnLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLog;
    }

    private void OnLog(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            errors.Add($"[{SceneManager.GetActiveScene().name}] {type}: {message}\n{stackTrace}");
        }
    }

    private IEnumerator Start()
    {
        Directory.CreateDirectory(outputFolder);
        Log($"Smoke test started ({Application.platform}, Unity {Application.unityVersion})");

        // Main menu and its options panel.
        yield return Load("Menu", 4f);
        yield return Capture("main-menu");
        Check(PlayerMusicManager.instance != null && PlayerMusicManager.instance.Current != null
              && PlayerMusicManager.instance.Current.loop, "menu music was started and loops");

        SettingsMenu options = FindObjectOfType<SettingsMenu>(true);
        Check(options != null, "options panel exists");
        if (options != null)
        {
            options.gameObject.SetActive(true);
            yield return WaitRealtime(1f);
            yield return Capture("options");
            options.gameObject.SetActive(false);
        }

        // Every level, with the pause menu in the first one.
        foreach (string level in Levels)
        {
            yield return Load(level, 6f);
            Check(GameObject.FindGameObjectWithTag("Player") != null, $"{level} has a player");
            Check(PlayerSFXManager.instance != null, $"{level} has the player SFX channel");
            yield return Capture(level.ToLowerInvariant());

            if (level == Levels[0]) yield return PauseAndResume();
        }

        yield return Load("Credits", 3f);
        yield return Capture("credits");

        Finish();
    }

    private IEnumerator PauseAndResume()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        Check(pauseMenu != null, "level has a pause menu");
        if (pauseMenu == null) yield break;

        pauseMenu.Pause();
        yield return WaitRealtime(1.5f);
        Check(Time.timeScale == 0f && PauseMenu.IsPaused, "pausing stops time");
        yield return Capture("pause-menu");

        pauseMenu.Resume();
        yield return null;
        Check(Time.timeScale == 1f && !PauseMenu.IsPaused, "resuming restarts time");
    }

    private IEnumerator Load(string scene, float settleSeconds)
    {
        Log($"Loading {scene}");
        AsyncOperation loading = SceneManager.LoadSceneAsync(scene);
        while (!loading.isDone) yield return null;
        yield return WaitRealtime(settleSeconds);
        Check(SceneManager.GetActiveScene().name == scene, $"{scene} is the active scene");
    }

    private IEnumerator Capture(string label)
    {
        yield return new WaitForEndOfFrame();
        Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
        string file = Path.Combine(outputFolder, $"{++screenshotIndex:00}-{label}.png");
        File.WriteAllBytes(file, shot.EncodeToPNG());
        Destroy(shot);
        Log($"Screenshot {file}");
    }

    private static IEnumerator WaitRealtime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }

    private void Check(bool condition, string description)
    {
        Log($"{(condition ? "PASS" : "FAIL")} {description}");
        if (!condition) errors.Add($"Check failed: {description}");
    }

    private void Log(string line)
    {
        report.AppendLine(line);
        Debug.Log($"[SmokeTest] {line}");
    }

    private void Finish()
    {
        Log(errors.Count == 0 ? "RESULT: PASSED" : $"RESULT: FAILED ({errors.Count} problems)");
        foreach (string error in errors) report.AppendLine(error);
        File.WriteAllText(Path.Combine(outputFolder, "smoke-results.txt"), report.ToString());
        Application.Quit(errors.Count == 0 ? 0 : 1);
    }
}
