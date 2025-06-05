namespace FsmNet
{
    using System;

    /// <summary>
    /// Represents a state in a finite state machine defined by an enumeration.
    /// </summary>
    /// <typeparam name="TEnum">State Enum Type</typeparam>
    public class EnumState<TEnum> : IState where TEnum : struct, Enum
    {
        /// <summary>
        /// The value of the enumeration representing this state.
        /// </summary>
        public TEnum Value { get; }

        /// <summary>
        /// The name of the state, derived from the enumeration value.
        /// </summary>
        public string Name => Value.ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumState{TEnum}"/> class with the specified enumeration value.
        /// </summary>
        /// <param name="value">Enum state value</param>
        public EnumState(TEnum value) => Value = value;

        /// <summary>
        /// Returns a string representation of the state, which is its name.
        /// </summary>
        /// <returns>String representation of state</returns>
        public override string ToString() => Name;
    }
}
