using Horror.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Voices placed in the level.</summary>
public class WorldVoiceManager : AudioChannel<WorldVoiceManager>
{
    [Header("Clips")]
    [SerializeField, FormerlySerializedAs("v_ghost")] private AudioClip ghost;

    protected override void RegisterClips(SoundBank<AudioClip> clips)
    {
        clips.Add("ghost", ghost);
    }

    public AudioSource PlayVoice(string soundName) => Play(soundName);
}
