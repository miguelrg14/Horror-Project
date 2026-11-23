using Horror.Core;
using NUnit.Framework;

namespace Horror.Tests
{
    public class VolumeMathTests
    {
        [TestCase(1f, 0f)]
        [TestCase(0.5f, -6.0206f)]
        [TestCase(0.1f, -20f)]
        [TestCase(0.01f, -40f)]
        public void LinearToDecibels_UsesTwentyLog10(float linear, float expectedDb)
        {
            Assert.AreEqual(expectedDb, VolumeMath.LinearToDecibels(linear), 0.001f);
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        [TestCase(float.NaN)]
        public void SilentOrInvalidValues_MapToMinusEighty(float linear)
        {
            Assert.AreEqual(VolumeMath.MinDecibels, VolumeMath.LinearToDecibels(linear));
        }

        [Test]
        public void ValuesAboveOne_AreCappedAtZeroDecibels()
        {
            Assert.AreEqual(0f, VolumeMath.LinearToDecibels(3f));
        }

        [TestCase(0.25f)]
        [TestCase(0.75f)]
        [TestCase(1f)]
        public void DecibelsToLinear_IsTheInverse(float linear)
        {
            float db = VolumeMath.LinearToDecibels(linear);
            Assert.AreEqual(linear, VolumeMath.DecibelsToLinear(db), 0.0001f);
        }

        [Test]
        public void HalfTheSlider_IsStillClearlyAudible()
        {
            // With the original -80..0 dB slider, the middle position was -40 dB (almost silent).
            Assert.Greater(VolumeMath.LinearToDecibels(0.5f), -10f);
        }
    }
}
