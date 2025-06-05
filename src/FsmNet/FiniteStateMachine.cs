namespace FsmNet
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a finite state machine based on an enumeration type.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public class FiniteStateMachine<TState, TContext> : BaseStateMachine<TState, TContext> where TState : struct, Enum
    {
        private readonly EnumStateMachineDefinition<TState, TContext> _definition;
        private EnumState<TState> _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine{TState, TContext}"/> class with the specified state machine definition.
        /// </summary>
        /// <param name="definition"></param>
        public FiniteStateMachine(EnumStateMachineDefinition<TState, TContext> definition)
        {
            _definition = definition;
            _current = (EnumState<TState>)definition.InitialState;
        }

        /// <summary>
        /// Gets the current state of the finite state machine.
        /// </summary>
        public override TState Current => _current.Value;

        /// <summary>
        /// Gets the definition of the finite state machine.
        /// </summary>
        /// <param name="target">Target state</param>
        /// <param name="context">Context object</param>
        /// <returns>If transition to target state is allowed</returns>
        public override bool CanTransitionTo(TState target, TContext context) =>
            _definition.Transitions.Any(t =>
                t.From.Name == _current.Name &&
                t.To.Name == target.ToString() &&
                t.Condition(context));

        /// <summary>
        /// Attempts to transition to the specified target state given the current context.
        /// </summary>
        /// <param name="target">Target state</param>
        /// <param name="context">Context object</param>
        /// <returns>If transtion to target state is successful</returns>
        public override bool TryTransitionTo(TState target, TContext context)
        {
            var transition = _definition.Transitions.FirstOrDefault(t =>
                t.From.Name == _current.Name &&
                t.To.Name == target.ToString() &&
                t.Condition(context));

            if (transition is null)
                return false;

            transition.SideEffect?.Invoke(context);
            _current = new EnumState<TState>(target);
            return true;
        }
    }
}
