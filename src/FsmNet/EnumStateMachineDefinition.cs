namespace FsmNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a state machine definition based on an enumeration type.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class EnumStateMachineDefinition<TEnum, TContext> : IStateMachineDefinition<TContext> where TEnum : struct, Enum
    {
        /// <summary>
        /// The type of work item this state machine is associated with.
        /// </summary>
        public string WorkItemType { get; }

        /// <summary>
        /// A collection of all states in the state machine, represented as <see cref="IState"/> instances.
        /// </summary>
        public IEnumerable<IState> States { get; }

        /// <summary>
        /// A collection of transitions between states, represented as <see cref="ITransition{TContext}"/> instances.
        /// </summary>
        public IEnumerable<ITransition<TContext>> Transitions { get; }

        /// <summary>
        /// The initial state of the state machine, represented as an <see cref="IState"/> instance.
        /// </summary>
        public IState InitialState { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="EnumStateMachineDefinition{TEnum, TContext}"/> class.
        /// </summary>
        /// <param name="workItemType"></param>
        /// <param name="states"></param>
        /// <param name="transitions"></param>
        /// <param name="initialState"></param>
        public EnumStateMachineDefinition(string workItemType, IEnumerable<TEnum> states, IEnumerable<ITransition<TContext>> transitions, TEnum initialState)
        {
            WorkItemType = workItemType;
            States = states.Select(s => new EnumState<TEnum>(s)).ToList();
            Transitions = transitions;
            InitialState = new EnumState<TEnum>(initialState);
        }
    }
}
