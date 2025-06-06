# API Overview

## Namespaces
- SharpFsm
- SharpFsm.Serialization

## Core Types

1. `FiniteStateMachine<TState, TContext>`: Represents a runtime finite state machine.
    - Type Parameters:
        - `TState`: Enum type representing states.
        - `TContext`: Type for contextual data passed to conditions and side effects.
    - Properties:
        - `TState Current` The current state of the FSM.
    - Methods:
        - `bool CanTransitionTo(TState target, TContext context)` Checks if a transition to the target state is possible given the context.
        - `bool TryTransitionTo(TState target, TContext context)` Attempts to transition to the target state. Returns true if successful.

2. `FiniteStateMachineBuilder<TState, TContext>`: Fluent builder for defining FSMs
    - Type Parameters:
        - `TState`: Enum type representing states.
        - `TContext`: Type for contextual data passed to conditions and side effects.
    - Static Methods:
	    - `FiniteStateMachineBuilder<TState, TContext> Create(string entityType)`: Creates a new instance of the `FiniteStateMachineBuilder` for the specified entity type.
	- Instance Methods:
	    - `FiniteStateMachineBuilder<TState, TContext> WithInitialState(TState state)`: Sets the initial state of the finite state machine. This method also adds the state to the list of states if it is not already present.
	    - `TransitionBuilder AddTransition(TState from, TState to)`: Adds a transition between two states in the finite state machine. This method also adds both states to the list of states if they are not already present.
	    - `EnumStateMachineDefinition<TState, TContext> Build()`: Builds the finite state machine definition. This method will throw an exception if the initial state has not been set.
	    - `SerializableStateMachine ToSerializable()`: Converts the current state machine builder to a serializable state machine definition.
	    - `FiniteStateMachineBuilder<TState, TContext> LoadFrom(SerializableStateMachine dto, TransitionRegistry<TContext> registry)`: Loads a finite state machine from a serializable state machine definition.
	- Nested Type:
	    - `TransitionBuilder`: Builder for defining transitions in the finite state machine.

3. `EnumStateMachineDefinition<TState, TContext>`: Describes the structure of an FSM (states, transitions, initial state).
    - Type Parameters:
        - `TState`: Enum type representing states.
        - `TContext`: Type for contextual data passed to conditions and side effects.
    - Properties:
	    - `string EntityType`: The type of work item this state machine is associated with.
	    - `IEnumerable<IState> States`: A collection of all states in the state machine, represented as `IState` instances.
	    - `IEnumerable<ITransition<TContext>> Transitions`: A collection of transitions between states, represented as `ITransition` instances.
	    - `IState InitialState`: The initial state of the state machine, represented as an `IState` instance.

4. `TransitionRegistry<TContext>`: Registry for reusable conditions and side effects.
    - Type Parameters:
        - `TContext`: Type for contextual data passed to conditions and side effects.
    - Properties:
	    - `Dictionary<string, Func<TContext, bool>> Conditions`: Stores conditions that can be used to evaluate whether a transition can occur.
	    - `Dictionary<string, Action<TContext>> SideEffects`: Stores side effects that can be executed when a transition occurs.
    - Methods:
	    - `void RegisterCondition(string name, Func<TContext, bool> fn)`: Registers a condition with a name that can be used to evaluate transitions.
	    - `void RegisterSideEffect(string name, Action<TContext> fn)`: Registers a side effect with a name that can be executed when a transition occurs.

5. `ITransition<TContext>`: Represents a transition between two states.
    - Type Parameters:
        - `TContext`: Type for contextual data passed to conditions and side effects.
    - Properties:
	    - `IState From`: Gets the source state of the transition.
	    - `IState To`: Gets the target state of the transition.
	    - `Func<TContext, bool> Condition`: Gets the condition that must be met for this transition to occur.
	    - `Action<TContext> SideEffect`: Gets an optional side effect that occurs when the transition is taken.
	    - `string ConditionName`: Gets the name of the condition that triggers this transition, if any.
	    - `string SideEffectName`: Gets the name of the side effect that occurs when this transition is taken, if any.

6. `IState`: Represents a state in the FSM.
	- Property:
	    - `string Name`: Gets the name of the state.

7. `SerializableStateMachine`:
	- Properties:
	    - `string EntityType`: The type of entity this state machine is associated with.
	    - `string InitialState`: The initial state of the state machine.
	    - `List<string> States`: A list of all states in the state machine.
	    - `List<SerializableTransition> Transitions`: A list of transitions between states.

8. `SerializableTransition`:
	- Properties:
	    - `string From`: The state this transition is from.
	    - `string To`: The state this transition is to.
	    - `string ConditionName`: The name of the condition that triggers this transition.
	    - `string SideEffectName`: The name of the side effect that occurs when this transition is taken.
