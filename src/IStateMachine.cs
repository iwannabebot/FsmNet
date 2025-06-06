namespace SharpFsm
{
    using System;

    /// <summary>
    /// Interface for a state machine that defines the core functionality for transitioning between states.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public interface IStateMachine<TState, TContext> where TState : struct, Enum
    {
        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        TState Current { get; }

        /// <summary>
        /// Checks if a transition to the specified target state is possible given the current context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CanTransitionTo(TState target, TContext context);

        /// <summary>
        /// Attempts to transition to the specified target state given the current context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        bool TryTransitionTo(TState target, TContext context);
    }
}
