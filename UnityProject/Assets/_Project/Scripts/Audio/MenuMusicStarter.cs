using UnityEngine;

/// <summary>Starts the looping main-menu theme.</summary>
public class MenuMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (PlayerMusicManager.instance == null) return;
        PlayerMusicManager.instance.PlayMusic("menu");
        PlayerMusicManager.instance.Loop();
    }
}
