using HorrorProject.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Returns to the main menu once the credits have been shown.</summary>
public class CreditsTimer : MonoBehaviour
{
    [SerializeField] private float duration = 10f;
    [SerializeField] private string nextScene = PauseMenu.MainMenuScene;

    private CountdownTimer timer;

    private void Start()
    {
        timer = new CountdownTimer(duration);
    }

    private void Update()
    {
        if (timer.Tick(Time.deltaTime)) SceneManager.LoadScene(nextScene);
    }
}
