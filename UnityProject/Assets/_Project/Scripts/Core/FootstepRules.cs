using System;

namespace Horror.Core
{
    /// <summary>When footsteps play and how each step is varied so the loop does not sound mechanical.</summary>
    public static class FootstepRules
    {
        public const float MinVolume = 0.8f;
        public const float MaxVolume = 1f;
        public const float MinPitch = 0.8f;
        public const float MaxPitch = 1.1f;
        private const float DeadZone = 0.01f;

        /// <summary>True when the movement input is not centred (also for diagonals like (1, -1)).</summary>
        public static bool IsMoving(float horizontal, float vertical)
        {
            return horizontal * horizontal + vertical * vertical > DeadZone * DeadZone;
        }

        public static (float volume, float pitch) RandomStep(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            float volume = Lerp(MinVolume, MaxVolume, (float)random.NextDouble());
            float pitch = Lerp(MinPitch, MaxPitch, (float)random.NextDouble());
            return (volume, pitch);
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
