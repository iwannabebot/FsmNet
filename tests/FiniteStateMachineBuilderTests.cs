using SharpFsm.Serialization;

namespace SharpFsm.UnitTests
{
    public class FiniteStateMachineBuilderTests
    {
        private enum TestState { Start, Middle, End }
        private class TestContext { public bool Flag { get; set; } }

        [Fact]
        public void Create_SetsEntityType()
        {
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity");
            var definition = builder.WithInitialState(TestState.Start).Build();
            Assert.Equal("TestEntity", definition.EntityType);
        }

        [Fact]
        public void WithInitialState_SetsInitialState()
        {
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity")
                .WithInitialState(TestState.Middle);
            var definition = builder.Build();
            Assert.Equal("Middle", definition.InitialState.Name);
        }

        [Fact]
        public void AddTransition_AddsStatesAndTransition()
        {
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity")
                .WithInitialState(TestState.Start)
                .AddTransition(TestState.Start, TestState.End)
                    .When(ctx => true, "Always")
                    .Done();

            var definition = builder.Build();
            var stateNames = new HashSet<string>();
            foreach (var state in definition.States)
                stateNames.Add(state.Name);

            Assert.Contains("Start", stateNames);
            Assert.Contains("End", stateNames);
            Assert.Single(definition.Transitions);
        }

        [Fact]
        public void TransitionBuilder_WhenAndWithSideEffect_SetCorrectly()
        {
            bool effectCalled = false;
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity")
                .WithInitialState(TestState.Start)
                .AddTransition(TestState.Start, TestState.End)
                    .When(ctx => ctx.Flag, "FlagTrue")
                    .WithSideEffect(ctx => effectCalled = true, "Effect")
                    .Done();

            var definition = builder.Build();
            var transition = Assert.Single(definition.Transitions);
            var context = new TestContext { Flag = true };
            Assert.True(transition.Condition(context));
            transition.SideEffect(context);
            Assert.True(effectCalled);
            Assert.Equal("FlagTrue", transition.ConditionName);
            Assert.Equal("Effect", transition.SideEffectName);
        }

        [Fact]
        public void Build_WithoutInitialState_Throws()
        {
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity");
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void ToSerializable_ProducesExpectedResult()
        {
            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity")
                .WithInitialState(TestState.Start)
                .AddTransition(TestState.Start, TestState.End)
                    .When(ctx => true, "Always")
                    .Done();

            var serializable = builder.ToSerializable();
            Assert.Equal("TestEntity", serializable.EntityType);
            Assert.Equal("Start", serializable.InitialState);
            Assert.Contains("Start", serializable.States);
            Assert.Contains("End", serializable.States);
            Assert.Single(serializable.Transitions);
            Assert.Equal("Always", serializable.Transitions[0].ConditionName);
        }

        [Fact]
        public void LoadFrom_LoadsStatesAndTransitions()
        {
            var registry = new TransitionRegistry<TestContext>();
            registry.RegisterCondition("Always", ctx => true);

            var dto = new SerializableStateMachine
            {
                EntityType = "TestEntity",
                InitialState = "Start",
                States = new List<string> { "Start", "End" },
                Transitions = new List<SerializableTransition>
            {
                new SerializableTransition
                {
                    From = "Start",
                    To = "End",
                    ConditionName = "Always"
                }
            }
            };

            var builder = FiniteStateMachineBuilder<TestState, TestContext>.Create("TestEntity")
                .LoadFrom(dto, registry);

            var definition = builder.Build();
            Assert.Equal("TestEntity", definition.EntityType);
            Assert.Equal("Start", definition.InitialState.Name);
            Assert.Single(definition.Transitions);
            Assert.Equal("End", Assert.Single(definition.Transitions).To.Name);
        }
    }
}