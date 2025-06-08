namespace SharpFsm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a definition of a state machine, which includes its states, transitions, and initial state.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public interface IStateMachineDefinition<TState, TContext> where TState : struct, Enum
    {
        /// <summary>
        /// Gets the type of work item this state machine is associated with.
        /// </summary>
        string EntityType { get; }

        /// <summary>
        /// Gets a collection of all states in the state machine.
        /// </summary>
        IEnumerable<IState> States { get; }

        /// <summary>
        /// Gets a collection of transitions between states in the state machine.
        /// </summary>
        IEnumerable<ITransition<TState, TContext>> Transitions { get; }

        /// <summary>
        /// Gets the initial state of the state machine.
        /// </summary>
        IState InitialState { get; }
    }
}
