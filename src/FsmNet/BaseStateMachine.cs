namespace FsmNet
{
    using System;

    /// <summary>
    /// Base class for state machines that defines the core functionality for transitioning between states.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public abstract class BaseStateMachine<TState, TContext> : IStateMachine<TState, TContext> where TState : struct, Enum
    {
        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        public virtual TState Current { get; protected set; }

        /// <summary>
        /// Checks if a transition to the specified target state is possible given the current context.
        /// </summary>
        /// <param name="target">Target State</param>
        /// <param name="context">Context</param>
        /// <returns>If transition is possible to target state</returns>
        public abstract bool CanTransitionTo(TState target, TContext context);

        /// <summary>
        /// Attempts to transition to the specified target state given the current context.
        /// </summary>
        /// <param name="target">Target State</param>
        /// <param name="context">Context</param>
        /// <returns>If transition is successful to target state</returns>
        public abstract bool TryTransitionTo(TState target, TContext context);
    }
}
