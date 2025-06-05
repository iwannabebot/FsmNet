namespace FsmNet
{
    /// <summary>
    /// Represents a state in a finite state machine.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        string Name { get; }
    }
}
