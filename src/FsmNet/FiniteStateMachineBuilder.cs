namespace FsmNet
{
    using FsmNet.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Builder for creating a finite state machine based on an enumeration type.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class FiniteStateMachineBuilder<TState, TContext> where TState : struct, Enum
    {
        private readonly string _workItemType;
        private TState? _initial;
        private readonly HashSet<TState> _states = new HashSet<TState>();
        private readonly List<ITransition<TContext>> _transitions = new List<ITransition<TContext>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachineBuilder{TState, TContext}"/> class with the specified work item type.
        /// </summary>
        /// <param name="workItemType"></param>
        private FiniteStateMachineBuilder(string workItemType) => _workItemType = workItemType;

        /// <summary>
        /// Creates a new instance of the <see cref="FiniteStateMachineBuilder{TState, TContext}"/> for the specified work item type.
        /// </summary>
        /// <param name="workItemType"></param>
        /// <returns></returns>
        public static FiniteStateMachineBuilder<TState, TContext> Create(string workItemType) => new FiniteStateMachineBuilder<TState, TContext>(workItemType);

        /// <summary>
        /// Sets the initial state of the finite state machine. This method also adds the state to the list of states if it is not already present.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public FiniteStateMachineBuilder<TState, TContext> WithInitialState(TState state)
        {
            _initial = state;
            _states.Add(state);
            return this;
        }

        /// <summary>
        /// Adds a transition between two states in the finite state machine. This method also adds both states to the list of states if they are not already present.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public TransitionBuilder AddTransition(TState from, TState to)
        {
            _states.Add(from);
            _states.Add(to);
            return new TransitionBuilder(this, from, to);
        }

        /// <summary>
        /// Builds the finite state machine definition. This method will throw an exception if the initial state has not been set.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public EnumStateMachineDefinition<TState, TContext> Build()
        {
            if (_initial is null) throw new InvalidOperationException("Initial state not specified.");
            return new EnumStateMachineDefinition<TState, TContext>(_workItemType, _states, _transitions, _initial.Value);
        }

        /// <summary>
        /// Converts the current state machine builder to a serializable state machine definition.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public SerializableStateMachine ToSerializable()
        {
            if (_initial is null) throw new InvalidOperationException("Initial state must be defined.");
            return new SerializableStateMachine
            {
                EntityType = _workItemType,
                InitialState = _initial.ToString(),
                States = _states.Select(s => s.ToString()).ToList(),
                Transitions = _transitions.Select(t => new SerializableTransition
                {
                    From = t.From.Name,
                    To = t.To.Name,
                    ConditionName = t.ConditionName,
                    SideEffectName = t.SideEffectName
                }).ToList()
            };
        }

        /// <summary>
        /// Builder for defining transitions in the finite state machine.
        /// </summary>
        public class TransitionBuilder
        {
            private readonly FiniteStateMachineBuilder<TState, TContext> _parent;
            private readonly TState _from;
            private readonly TState _to;
            private Func<TContext, bool> _condition = _ => true;
            private Action<TContext> _sideEffect = null;
            private string _conditionName;
            private string _sideEffectName;

            public TransitionBuilder(FiniteStateMachineBuilder<TState, TContext> parent, TState from, TState to)
            {
                _parent = parent;
                _from = from;
                _to = to;
            }

            public TransitionBuilder When(Func<TContext, bool> condition, string name = null)
            {
                _condition = condition;
                _conditionName = name;
                return this;
            }

            public TransitionBuilder WithSideEffect(Action<TContext> effect, string name = null)
            {
                _sideEffect = effect;
                _sideEffectName = name;
                return this;
            }

            public FiniteStateMachineBuilder<TState, TContext> Done()
            {
                _parent._transitions.Add(new EnumTransition<TState, TContext>(_from, _to, _condition, _sideEffect, _conditionName, _sideEffectName));
                return _parent;
            }

            public static implicit operator FiniteStateMachineBuilder<TState, TContext>(TransitionBuilder b) => b.Done();
        }

        /// <summary>
        /// Loads a finite state machine from a serializable state machine definition.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="registry"></param>
        /// <returns></returns>
        public FiniteStateMachineBuilder<TState, TContext> LoadFrom(SerializableStateMachine dto, TransitionRegistry<TContext> registry)
        {
            _initial = (TState)Enum.Parse(typeof(TState), dto.InitialState);
            foreach (var s in dto.States)
                _states.Add((TState)Enum.Parse(typeof(TState), s));

            foreach (var t in dto.Transitions)
            {
                var from = (TState)Enum.Parse(typeof(TState), t.From);
                var to = (TState)Enum.Parse(typeof(TState), t.To);
                var cond = t.ConditionName != null && registry.Conditions.TryGetValue(t.ConditionName, out var c) ? c : _ => true;
                var side = t.SideEffectName != null && registry.SideEffects.TryGetValue(t.SideEffectName, out var se) ? se : null;
                _transitions.Add(new EnumTransition<TState, TContext>(from, to, cond, side));
            }

            return this;
        }
    }
}
