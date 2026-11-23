using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Sound effects that belong to the level rather than to the player.</summary>
public class WorldSFXManager : AudioChannel<WorldSFXManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("sfx_openBag")] private AudioClip openBag;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("openBag", openBag);
    }

    public AudioSource PlaySFX(string soundName) => Play(soundName);
}
