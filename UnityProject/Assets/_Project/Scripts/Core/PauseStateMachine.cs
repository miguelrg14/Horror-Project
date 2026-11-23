namespace Horror.Core
{
    public enum PauseState
    {
        Playing,
        Paused,
        Options
    }

    /// <summary>
    /// States of the in-game pause menu. Escape pauses, resumes, or closes the options panel
    /// and goes back to the pause menu.
    /// </summary>
    public sealed class PauseStateMachine
    {
        public PauseState State { get; private set; } = PauseState.Playing;

        public bool IsPaused => State != PauseState.Playing;

        public PauseState PressEscape()
        {
            switch (State)
            {
                case PauseState.Playing: return Pause();
                case PauseState.Paused: return Resume();
                default: return CloseOptions();
            }
        }

        public PauseState Pause() => State = PauseState.Paused;

        public PauseState Resume() => State = PauseState.Playing;

        public PauseState OpenOptions() => State = IsPaused ? PauseState.Options : State;

        public PauseState CloseOptions() => State = State == PauseState.Options ? PauseState.Paused : State;
    }
}
