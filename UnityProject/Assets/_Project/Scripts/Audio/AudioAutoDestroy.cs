using UnityEngine;

/// <summary>
/// Removes a one-shot sound object once its clip has finished or been stopped. Looping sounds are kept.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioAutoDestroy : MonoBehaviour
{
    /// <summary>
    /// How long (real time) a sound may stay silent before it is treated as never started:
    /// a stopped-before-start sound, a missing clip, or a machine without an audio device.
    /// </summary>
    public const float StartGracePeriod = 0.25f;

    private AudioSource source;
    private bool hasStarted;
    private float createdAt;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        createdAt = Time.unscaledTime;
    }

    private void Update()
    {
        if (source.loop) return;

        if (source.isPlaying)
        {
            hasStarted = true;
            return;
        }

        // The sound is created and played on the same frame, so wait for it to start before
        // treating "not playing" as "finished".
        bool neverStarted = Time.unscaledTime - createdAt > StartGracePeriod;
        if (hasStarted || neverStarted || source.clip == null) Destroy(gameObject);
    }
}
