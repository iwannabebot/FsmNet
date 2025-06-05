namespace FsmNet.Serialization
{
    /// <summary>
    /// Represents a transition in a state machine that can be serialized.
    /// </summary>
    public class SerializableTransition
    {
        /// <summary>
        /// The state this transition is from.
        /// </summary>
        public string From { get; set; } = default;

        /// <summary>
        /// The state this transition is to.
        /// </summary>
        public string To { get; set; } = default;

        /// <summary>
        /// The name of the condition that triggers this transition.
        /// </summary>
        public string ConditionName { get; set; }

        /// <summary>
        /// The name of the side effect that occurs when this transition is taken.
        /// </summary>
        public string SideEffectName { get; set; }
    }
}
