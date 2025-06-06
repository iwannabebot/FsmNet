namespace SharpFsm.UnitTests
{
    public class EnumStateMachineDefinitionTests
    {
        private enum TestState { A, B, C }
        private class TestContext { }

        private class DummyTransition : ITransition<TestContext>
        {
            public IState From { get; }
            public IState To { get; }
            public Func<TestContext, bool> Condition { get; }
            public Action<TestContext> SideEffect { get; }
            public string ConditionName { get; }
            public string SideEffectName { get; }

            public DummyTransition(IState from, IState to)
            {
                From = from;
                To = to;
                Condition = _ => true;
                SideEffect = null;
                ConditionName = "Always";
                SideEffectName = null;
            }
        }

        [Fact]
        public void Constructor_AssignsPropertiesCorrectly()
        {
            var states = new[] { TestState.A, TestState.B };
            var transitions = new List<ITransition<TestContext>>
        {
            new DummyTransition(new EnumState<TestState>(TestState.A), new EnumState<TestState>(TestState.B))
        };
            var def = new EnumStateMachineDefinition<TestState, TestContext>(
                "TestEntity", states, transitions, TestState.A);

            Assert.Equal("TestEntity", def.EntityType);
            Assert.NotNull(def.States);
            Assert.NotNull(def.Transitions);
            Assert.NotNull(def.InitialState);
        }

        [Fact]
        public void States_AreEnumStateInstances_WithCorrectNames()
        {
            var states = new[] { TestState.A, TestState.B, TestState.C };
            var def = new EnumStateMachineDefinition<TestState, TestContext>(
                "TestEntity", states, new List<ITransition<TestContext>>(), TestState.B);

            var stateNames = new HashSet<string>();
            foreach (var state in def.States)
                stateNames.Add(state.Name);

            Assert.Contains("A", stateNames);
            Assert.Contains("B", stateNames);
            Assert.Contains("C", stateNames);
            Assert.Equal(3, stateNames.Count);
        }

        [Fact]
        public void InitialState_IsCorrect()
        {
            var def = new EnumStateMachineDefinition<TestState, TestContext>(
                "TestEntity", new[] { TestState.A, TestState.B }, new List<ITransition<TestContext>>(), TestState.B);

            Assert.Equal("B", def.InitialState.Name);
        }

        [Fact]
        public void Transitions_AreAssigned()
        {
            var transition = new DummyTransition(new EnumState<TestState>(TestState.A), new EnumState<TestState>(TestState.B));
            var transitions = new List<ITransition<TestContext>> { transition };
            var def = new EnumStateMachineDefinition<TestState, TestContext>(
                "TestEntity", new[] { TestState.A, TestState.B }, transitions, TestState.A);

            Assert.Single(def.Transitions);
            Assert.Equal("A", def.Transitions.GetEnumerator().Current?.From.Name ?? "A");
            Assert.Equal("B", def.Transitions.GetEnumerator().Current?.To.Name ?? "B");
        }
    }
}