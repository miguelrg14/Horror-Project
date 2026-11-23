using Horror.Core;
using NUnit.Framework;

namespace Horror.Tests
{
    public class PauseStateMachineTests
    {
        [Test]
        public void Escape_TogglesBetweenPlayingAndPaused()
        {
            var pause = new PauseStateMachine();

            Assert.AreEqual(PauseState.Paused, pause.PressEscape());
            Assert.IsTrue(pause.IsPaused);
            Assert.AreEqual(PauseState.Playing, pause.PressEscape());
            Assert.IsFalse(pause.IsPaused);
        }

        [Test]
        public void Escape_InOptions_GoesBackToThePauseMenu()
        {
            var pause = new PauseStateMachine();
            pause.Pause();
            pause.OpenOptions();

            Assert.AreEqual(PauseState.Options, pause.State);
            Assert.AreEqual(PauseState.Paused, pause.PressEscape());
            Assert.IsTrue(pause.IsPaused, "Closing the options must not resume the game.");
        }

        [Test]
        public void Options_CannotBeOpenedWhilePlaying()
        {
            var pause = new PauseStateMachine();

            Assert.AreEqual(PauseState.Playing, pause.OpenOptions());
        }

        [Test]
        public void Resume_FromOptions_ReturnsToTheGame()
        {
            var pause = new PauseStateMachine();
            pause.Pause();
            pause.OpenOptions();

            Assert.AreEqual(PauseState.Playing, pause.Resume());
        }

        [Test]
        public void CloseOptions_OutsideTheOptions_ChangesNothing()
        {
            var pause = new PauseStateMachine();
            pause.Pause();

            Assert.AreEqual(PauseState.Paused, pause.CloseOptions());
        }
    }
}
