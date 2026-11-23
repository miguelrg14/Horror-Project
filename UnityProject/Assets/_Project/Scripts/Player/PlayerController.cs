using Horror.Core;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    InputPlayer inputPlayer;
    [SerializeField]
    private float speed = 10f;
    private Vector3 velocity;
    [SerializeField]
    private Rigidbody rb;
    public bool isMoving;

    [Header("Walking properties")]
    [SerializeField] private AudioSource stepAudio;

    private readonly System.Random stepRandom = new System.Random();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movement: input on the XZ plane, clamped so diagonals are not faster.
        velocity = new Vector3(inputPlayer.x, 0, inputPlayer.y);
        velocity = Vector3.ClampMagnitude(velocity, 1) * speed;
        // Z is forward, X is sideways and Y keeps the current vertical speed (gravity).
        rb.velocity = transform.right * velocity.x +
                      transform.up * rb.velocity.y +
                      transform.forward * velocity.z;

        // Diagonals such as (1, -1) also count as moving.
        isMoving = FootstepRules.IsMoving(inputPlayer.x, inputPlayer.y);

        UpdateFootsteps();
    }

    /// <summary>Loops the footstep clip while walking, varying volume and pitch on every step.</summary>
    private void UpdateFootsteps()
    {
        if (stepAudio == null) return;

        if (isMoving && !stepAudio.isPlaying)
        {
            (float volume, float pitch) = FootstepRules.RandomStep(stepRandom);
            stepAudio.volume = volume;
            stepAudio.pitch = pitch;
            stepAudio.Play();
        }
        else if (!isMoving && stepAudio.isPlaying)
        {
            stepAudio.Stop();
        }
    }
}
