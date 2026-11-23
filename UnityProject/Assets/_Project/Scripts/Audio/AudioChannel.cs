using Horror.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Base class for the game's audio channels (player SFX, music, UI, voices and world sounds).
/// Every channel routes its sounds through its own AudioMixer group, so the options menu can set
/// each volume independently.
///
/// Each sound is played on a short-lived object created from <see cref="voiceTemplate"/> (an
/// AudioSource set to the channel's mixer group plus an <see cref="AudioAutoDestroy"/>). Several sounds
/// can overlap, and each object removes itself when its clip ends; looping sounds live until stopped.
/// </summary>
public abstract class AudioChannel<T> : MonoBehaviour where T : AudioChannel<T>
{
    /// <summary>
    /// Channel in the current scene. Kept lower-case because the gameplay scripts written by the rest
    /// of the team call <c>PlayerSFXManager.instance.PlaySFX(...)</c>.
    /// </summary>
    public static T instance;

    [Tooltip("Prefab with an AudioSource routed to this channel's mixer group and an AudioAutoDestroy.")]
    [SerializeField, FormerlySerializedAs("referenceSoundObject")]
    private GameObject voiceTemplate;

    private readonly SoundBank<AudioClip> bank = new SoundBank<AudioClip>();

    /// <summary>Last sound started on this channel (null once it has finished).</summary>
    public AudioSource Current { get; private set; }

    /// <summary>Registers the channel's clips with the names used by gameplay code.</summary>
    protected abstract void RegisterClips(SoundBank<AudioClip> clips);

    protected virtual void Awake()
    {
        instance = this as T;
        RegisterClips(bank);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public bool HasSound(string soundName) => bank.TryGet(soundName, out _);

    /// <summary>Plays a registered sound once. Returns its AudioSource, or null if the name is unknown.</summary>
    public AudioSource Play(string soundName)
    {
        if (!bank.TryGet(soundName, out AudioClip clip))
        {
            if (bank.IsKnown(soundName)) return null; // registered on purpose without a clip
            Debug.LogWarning($"{GetType().Name}: there is no sound called '{soundName}'.", this);
            return null;
        }
        return Play(clip);
    }

    public AudioSource Play(AudioClip clip)
    {
        if (clip == null) return null;

        GameObject voice = voiceTemplate != null
            ? Instantiate(voiceTemplate, transform)
            : new GameObject(clip.name, typeof(AudioSource), typeof(AudioAutoDestroy));
        voice.transform.SetParent(transform, false);
        voice.name = clip.name;

        AudioSource source = voice.GetComponent<AudioSource>();
        source.clip = clip;
        source.loop = false; // one-shot by default; Loop() opts in
        source.Play();

        Current = source;
        return source;
    }

    /// <summary>Makes the last sound loop until <see cref="Stop()"/> is called.</summary>
    public void Loop()
    {
        if (Current != null) Current.loop = true;
    }

    public void NoLoop()
    {
        if (Current != null) Current.loop = false;
    }

    /// <summary>Stops and removes the last sound started on this channel.</summary>
    public void Stop() => Stop(Current);

    public static void Stop(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        Destroy(source.gameObject);
    }
}
