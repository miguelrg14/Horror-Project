using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Music tracks (menu theme, chase, tension pad and ending).</summary>
public class PlayerMusicManager : AudioChannel<PlayerMusicManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("music_menu")] private AudioClip menu;
    [SerializeField, FormerlySerializedAs("music_persecution")] private AudioClip persecution;
    [SerializeField, FormerlySerializedAs("music_terrorPad")] private AudioClip terrorPad;
    [SerializeField, FormerlySerializedAs("music_end")] private AudioClip ending;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("menu", menu);
        clips.Add("persecution", persecution);
        clips.Add("terrorPad", terrorPad);
        clips.Add("end", ending);
        // Requested by InputPlayer when a level starts; the ambience is played by the Player prefab's own AudioSource.
        clips.Add("backgroundAmbient", null);
    }

    public AudioSource PlayMusic(string soundName) => Play(soundName);
}
