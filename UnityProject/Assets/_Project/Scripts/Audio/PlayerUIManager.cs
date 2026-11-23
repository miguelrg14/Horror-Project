using Horror.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Interface sounds (menus and buttons).</summary>
public class PlayerUIManager : AudioChannel<PlayerUIManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("ui_enterMenu")] private AudioClip enterMenu;
    [SerializeField, FormerlySerializedAs("ui_button1")] private AudioClip button1;
    [SerializeField, FormerlySerializedAs("ui_button2")] private AudioClip button2;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("enterMenu", enterMenu);
        clips.Add("button1", button1);
        clips.Add("button2", button2);
    }

    public AudioSource PlayUI(string soundName) => Play(soundName);

    // Called from the buttons' OnClick events.
    public void PlayButton1() => PlayUI("button1");

    public void PlayButton2() => PlayUI("button2");
}
