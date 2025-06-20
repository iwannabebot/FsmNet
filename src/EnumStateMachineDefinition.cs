﻿namespace SharpFsm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a state machine definition based on an enumeration type.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public class EnumStateMachineDefinition<TState, TContext> : IStateMachineDefinition<TState, TContext> where TState : struct, Enum
    {
        /// <summary>
        /// The type of work item this state machine is associated with.
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// A collection of all states in the state machine, represented as <see cref="IState"/> instances.
        /// </summary>
        public IEnumerable<IState> States { get; }

        /// <summary>
        /// A collection of transitions between states, represented as <see cref="ITransition{TState, TContext}"/> instances.
        /// </summary>
        public IEnumerable<ITransition<TState, TContext>> Transitions { get; }

        /// <summary>
        /// The initial state of the state machine, represented as an <see cref="IState"/> instance.
        /// </summary>
        public IState InitialState { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="EnumStateMachineDefinition{TEnum, TContext}"/> class.
        /// </summary>
        /// <param name="entityType">Entity type name, example: Order, Bug, etc.</param>
        /// <param name="states"></param>
        /// <param name="transitions"></param>
        /// <param name="initialState"></param>
        public EnumStateMachineDefinition(string entityType, IEnumerable<TState> states, IEnumerable<ITransition<TState, TContext>> transitions, TState initialState)
        {
            EntityType = entityType;
            States = states.Select(s => new EnumState<TState>(s)).ToList();
            Transitions = transitions;
            InitialState = new EnumState<TState>(initialState);
        }
    }
}
