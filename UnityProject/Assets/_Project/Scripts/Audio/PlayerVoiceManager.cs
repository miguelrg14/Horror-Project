using HorrorProject.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Voices heard by the player.</summary>
public class PlayerVoiceManager : AudioChannel<PlayerVoiceManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("v_ghostOnBack")] private AudioClip ghostOnBack;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("ghostOnBack", ghostOnBack);
    }

    public AudioSource PlayVoice(string soundName) => Play(soundName);
}
