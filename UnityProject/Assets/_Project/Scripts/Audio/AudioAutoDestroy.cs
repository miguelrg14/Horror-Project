using UnityEngine;

/// <summary>Removes a one-shot sound object once its clip has finished. Looping sounds are kept.</summary>
[RequireComponent(typeof(AudioSource))]
public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource source;
    private bool hasStarted;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (source.isPlaying)
        {
            hasStarted = true;
            return;
        }

        // Wait until the sound has actually started: it is created and played on the same frame.
        bool finished = hasStarted && !source.loop;
        if (finished || source.clip == null) Destroy(gameObject);
    }
}
