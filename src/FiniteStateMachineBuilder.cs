namespace SharpFsm
{
    using SharpFsm.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Builder for creating a finite state machine based on an enumeration type.
    /// </summary>
    /// <typeparam name="TState">State Enum Type</typeparam>
    /// <typeparam name="TContext">Context Type</typeparam>
    public class FiniteStateMachineBuilder<TState, TContext> where TState : struct, Enum
    {
        private readonly string _entityType;
        private TState? _initial;
        private TransitionRegistry<TContext> _registry;
        private readonly HashSet<TState> _states = new HashSet<TState>();
        private readonly List<ITransition<TContext>> _transitions = new List<ITransition<TContext>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachineBuilder{TState, TContext}"/> class with the specified entity type.
        /// </summary>
        /// <param name="entityType"></param>
        private FiniteStateMachineBuilder(string entityType) => _entityType = entityType;

        /// <summary>
        /// Creates a new instance of the <see cref="FiniteStateMachineBuilder{TState, TContext}"/> for the specified entity type.
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
        /// Sets the transition registry for the finite state machine. The registry is used to manage conditions and side effects for transitions.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public FiniteStateMachineBuilder<TState, TContext> WithRegistry(TransitionRegistry<TContext> registry)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));
            _registry = registry;
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
            return new EnumStateMachineDefinition<TState, TContext>(_entityType, _states, _transitions, _initial.Value);
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
                EntityType = _entityType,
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

            /// <summary>
            /// Initializes a new instance of the <see cref="TransitionBuilder"/> class with the specified parent builder and states.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            public TransitionBuilder(FiniteStateMachineBuilder<TState, TContext> parent, TState from, TState to)
            {
                _parent = parent;
                _from = from;
                _to = to;
            }

            /// <summary>
            /// Defines a condition for the transition. The condition is a function that takes the context and returns a boolean indicating whether the transition can occur.
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public TransitionBuilder When(Func<TContext, bool> condition, string name = null)
            {
                _condition = condition;
                _conditionName = name;
                return this;
            }

            /// <summary>
            /// Defines a condition for the transition by referencing a condition name from the registry. This allows for reusing conditions defined elsewhere in the state machine.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public TransitionBuilder When(string name)
            {
                if(_parent._registry == null)
                    throw new InvalidOperationException("Transition registry is not set. Use WithRegistry to set it before defining transitions.");
                if (_parent._registry.Conditions.TryGetValue(name, out var condition))
                {
                    _condition = condition;
                    _conditionName = name;
                }
                else
                {
                    throw new ArgumentException($"Condition '{name}' not found in registry.");
                }
                return this;
            }

            /// <summary>
            /// Defines a side effect for the transition. The side effect is an action that takes the context and is executed when the transition occurs.
            /// </summary>
            /// <param name="effect"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public TransitionBuilder WithSideEffect(Action<TContext> effect, string name = null)
            {
                _sideEffect = effect;
                _sideEffectName = name;
                return this;
            }

            /// <summary>
            /// Defines a side effect for the transition by referencing a side effect name from the registry. This allows for reusing side effects defined elsewhere in the state machine.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public TransitionBuilder WithSideEffect(string name)
            {
                if (_parent._registry == null)
                    throw new InvalidOperationException("Transition registry is not set. Use WithRegistry to set it before defining transitions.");
                if (_parent._registry.SideEffects.TryGetValue(name, out var sideEffect))
                {
                    _sideEffect = sideEffect;
                    _sideEffectName = name;
                }
                else
                {
                    throw new ArgumentException($"Side effect '{name}' not found in registry.");
                }
                return this;
            }

            /// <summary>
            /// Finalizes the transition definition and adds it to the parent finite state machine builder's list of transitions.
            /// </summary>
            /// <returns></returns>
            public FiniteStateMachineBuilder<TState, TContext> Done()
            {
                _parent._transitions.Add(new EnumTransition<TState, TContext>(_from, _to, _condition, _sideEffect, _conditionName, _sideEffectName));
                return _parent;
            }

            /// <summary>
            /// Implicitly converts the TransitionBuilder to a FiniteStateMachineBuilder, allowing for method chaining.
            /// </summary>
            /// <param name="b"></param>
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
