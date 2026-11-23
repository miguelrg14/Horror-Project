using System;
using Horror.Core;
using NUnit.Framework;

namespace Horror.Tests
{
    public class FootstepRulesTests
    {
        [TestCase(1f, 0f)]
        [TestCase(0f, -1f)]
        [TestCase(1f, -1f)] // the original check (x + y != 0) treated this diagonal as standing still
        [TestCase(-0.5f, 0.5f)]
        public void AnyDirection_CountsAsMoving(float x, float y)
        {
            Assert.IsTrue(FootstepRules.IsMoving(x, y));
        }

        [TestCase(0f, 0f)]
        [TestCase(0.001f, -0.001f)]
        public void CentredInput_IsNotMoving(float x, float y)
        {
            Assert.IsFalse(FootstepRules.IsMoving(x, y));
        }

        [Test]
        public void RandomStep_StaysWithinTheConfiguredRanges()
        {
            var random = new Random(1234);
            for (int i = 0; i < 1000; i++)
            {
                (float volume, float pitch) = FootstepRules.RandomStep(random);
                Assert.That(volume, Is.InRange(FootstepRules.MinVolume, FootstepRules.MaxVolume));
                Assert.That(pitch, Is.InRange(FootstepRules.MinPitch, FootstepRules.MaxPitch));
            }
        }

        [Test]
        public void RandomStep_VariesBetweenSteps()
        {
            var random = new Random(42);
            var first = FootstepRules.RandomStep(random);
            var second = FootstepRules.RandomStep(random);

            Assert.AreNotEqual(first, second);
        }
    }
}
