using SharpFsm.Serialization;

namespace SharpFsm.UnitTests
{
    public class TransitionRegistryTests
    {
        public class TicketContext
        {
            public bool IsAgentAssigned { get; set; }
            public bool IsCustomerConfirmed { get; set; }
        }

        [Fact]
        public void TransitionRegistry_RegistersAndRetrievesCondition()
        {
            var registry = new TransitionRegistry<TicketContext>();
            registry.RegisterCondition("TestCondition", ctx => ctx.IsAgentAssigned);

            Assert.True(registry.Conditions.ContainsKey("TestCondition"));
            var context = new TicketContext { IsAgentAssigned = true };
            Assert.True(registry.Conditions["TestCondition"](context));
        }

        [Fact]
        public void TransitionRegistry_RegistersAndRetrievesSideEffect()
        {
            var registry = new TransitionRegistry<TicketContext>();
            bool effectCalled = false;
            registry.RegisterSideEffect("TestEffect", ctx => effectCalled = true);

            Assert.True(registry.SideEffects.ContainsKey("TestEffect"));
            registry.SideEffects["TestEffect"](new TicketContext());
            Assert.True(effectCalled);
        }
    }
}