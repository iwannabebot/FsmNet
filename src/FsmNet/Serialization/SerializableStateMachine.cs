namespace FsmNet.Serialization
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a serializable state machine definition.
    /// </summary>
    public class SerializableStateMachine
    {
        /// <summary>
        /// The type of entity this state machine is associated with.
        /// </summary>
        public string EntityType { get; set; } = default;

        /// <summary>
        /// The initial state of the state machine.
        /// </summary>
        public string InitialState { get; set; } = default;

        /// <summary>
        /// A list of all states in the state machine.
        /// </summary>
        public List<string> States { get; set; } = new List<string>();

        /// <summary>
        /// A list of transitions between states.
        /// </summary>
        public List<SerializableTransition> Transitions { get; set; } = new List<SerializableTransition>();
    }
}
