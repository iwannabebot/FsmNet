using SharpFsm.Serialization;

namespace SharpFsm.UnitTests
{
    public class TransitionRegistryTests
    {
        public enum TestState
        {
            Start,
            End
        }
        public class TicketContext
        {
            public bool IsAgentAssigned { get; set; }
            public bool IsCustomerConfirmed { get; set; }
        }

        [Fact]
        public void TransitionRegistry_RegistersAndRetrievesCondition()
        {
            var registry = new TransitionRegistry<TestState, TicketContext>();
            registry.RegisterCondition("TestCondition", ctx => ctx.IsAgentAssigned);

            Assert.True(registry.Conditions.ContainsKey("TestCondition"));
            var context = new TicketContext { IsAgentAssigned = true };
            Assert.True(registry.Conditions["TestCondition"](context));
        }

        [Fact]
        public void TransitionRegistry_RegistersAndRetrievesSideEffect()
        {
            var registry = new TransitionRegistry<TestState, TicketContext>();
            bool effectCalled = false;
            registry.RegisterSideEffect("TestEffect", (ctx, _, _) => effectCalled = true);

            Assert.True(registry.SideEffects.ContainsKey("TestEffect"));
            registry.SideEffects["TestEffect"](new TicketContext(), TestState.Start, TestState.End);
            Assert.True(effectCalled);
        }
    }
}