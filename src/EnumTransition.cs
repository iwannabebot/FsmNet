namespace SharpFsm
{
    using System;

    /// <summary>
    /// Represents a transition in a finite state machine defined by an enumeration.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public class EnumTransition<TState, TContext> : ITransition<TState, TContext> where TState : struct, Enum
    {
        /// <summary>
        /// The state this transition is from, represented as an <see cref="IState"/> instance.
        /// </summary>
        public IState From { get; }

        /// <summary>
        /// The state this transition is to, represented as an <see cref="IState"/> instance.
        /// </summary>
        public IState To { get; }

        /// <summary>
        /// The condition that must be met for this transition to occur, represented as a function that takes the context and returns a boolean.
        /// </summary>
        public Func<TContext, bool> Condition { get; }

        /// <summary>
        /// An optional side effect that occurs when the transition is taken, represented as an action that takes the context.
        /// </summary>
        public Action<TContext, TState, TState> SideEffect { get; }

        /// <summary>
        /// The name of the condition that triggers this transition, if any.
        /// </summary>
        public string ConditionName { get; }

        /// <summary>
        /// The name of the side effect that occurs when this transition is taken, if any.
        /// </summary>
        public string SideEffectName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTransition{TEnum, TContext}"/> class with the specified parameters.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="condition"></param>
        /// <param name="effect"></param>
        /// <param name="conditionName"></param>
        /// <param name="sideEffectName"></param>
        public EnumTransition(TState from, TState to, Func<TContext, bool> condition = null, Action<TContext, TState, TState> effect = null, string conditionName = null, string sideEffectName = null)
        {
            From = new EnumState<TState>(from);
            To = new EnumState<TState>(to);
            Condition = condition;
            SideEffect = effect;
            ConditionName = conditionName;
            SideEffectName = sideEffectName;
        }
    }
}
