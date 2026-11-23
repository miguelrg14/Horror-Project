using System;

namespace Horror.Core
{
    /// <summary>
    /// Converts between the 0-1 values shown on the volume sliders and the decibels an AudioMixer expects.
    /// A linear slider mapped straight to -80..0 dB sounds silent for most of its range.
    /// </summary>
    public static class VolumeMath
    {
        public const float MinDecibels = -80f;
        public const float MinLinear = 0.0001f; // 20 * log10(0.0001) = -80 dB

        public static float LinearToDecibels(float linear)
        {
            if (float.IsNaN(linear) || linear <= MinLinear) return MinDecibels;
            if (linear >= 1f) return 0f;
            return (float)(20.0 * Math.Log10(linear));
        }

        public static float DecibelsToLinear(float decibels)
        {
            if (float.IsNaN(decibels) || decibels <= MinDecibels) return 0f;
            if (decibels >= 0f) return 1f;
            return (float)Math.Pow(10.0, decibels / 20.0);
        }
    }
}
