namespace FsmNet
{
    using System;

    /// <summary>
    /// Represents a transition between two states in a finite state machine.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ITransition<TContext>
    {
        /// <summary>
        /// Gets the source state of the transition.
        /// </summary>
        IState From { get; }

        /// <summary>
        /// Gets the target state of the transition.
        /// </summary>
        IState To { get; }

        /// <summary>
        /// Gets the condition that must be met for this transition to occur.
        /// </summary>
        Func<TContext, bool> Condition { get; }

        /// <summary>
        /// Gets an optional side effect that occurs when the transition is taken.
        /// </summary>
        Action<TContext> SideEffect { get; }

        /// <summary>
        /// Gets the name of the condition that triggers this transition, if any.
        /// </summary>
        string ConditionName { get; }

        /// <summary>
        /// Gets the name of the side effect that occurs when this transition is taken, if any.
        /// </summary>
        string SideEffectName { get; }
    }
}
