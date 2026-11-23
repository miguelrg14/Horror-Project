using System;
using HorrorProject.Core;
using NUnit.Framework;

namespace HorrorProject.Tests
{
    public class CountdownTimerTests
    {
        [Test]
        public void Tick_ReportsTheFinishOnlyOnce()
        {
            var timer = new CountdownTimer(1f);

            Assert.IsFalse(timer.Tick(0.6f));
            Assert.IsTrue(timer.Tick(0.6f));
            Assert.IsFalse(timer.Tick(0.6f), "Loading the next scene must not be requested every frame.");
            Assert.IsTrue(timer.IsFinished);
            Assert.AreEqual(0f, timer.Remaining);
        }

        [Test]
        public void ZeroOrNegativeDeltas_DoNotAdvance()
        {
            var timer = new CountdownTimer(2f);
            timer.Tick(0f);
            timer.Tick(-1f);

            Assert.AreEqual(2f, timer.Remaining);
        }

        [Test]
        public void Reset_StartsAgain()
        {
            var timer = new CountdownTimer(0.5f);
            timer.Tick(1f);
            timer.Reset();

            Assert.IsFalse(timer.IsFinished);
            Assert.AreEqual(0.5f, timer.Remaining);
        }

        [Test]
        public void NegativeDuration_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CountdownTimer(-1f));
        }
    }
}
