using System;

namespace Horror.Core
{
    /// <summary>Counts down once and reports the frame it reaches zero (used by the credits screen).</summary>
    public sealed class CountdownTimer
    {
        public CountdownTimer(float duration)
        {
            if (duration < 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            Duration = duration;
            Remaining = duration;
        }

        public float Duration { get; }
        public float Remaining { get; private set; }
        public bool IsFinished => Remaining <= 0f;

        /// <summary>Advances the timer. Returns true only on the tick that finishes it.</summary>
        public bool Tick(float deltaTime)
        {
            if (IsFinished || deltaTime <= 0f) return false;
            Remaining = Math.Max(0f, Remaining - deltaTime);
            return IsFinished;
        }

        public void Reset() => Remaining = Duration;
    }
}
