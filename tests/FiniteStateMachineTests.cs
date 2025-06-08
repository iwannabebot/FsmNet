using SharpFsm.Serialization;
using SharpFsm;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SharpFsm.UnitTests
{
    public class FiniteStateMachineTests
    {
        public class TicketContext
        {
            public bool IsAgentAssigned { get; set; }
            public bool IsCustomerConfirmed { get; set; }
        }

        public enum TicketState
        {
            Open,
            InProgress,
            Resolved,
            Closed
        }

        private static TransitionRegistry<TicketState, TicketContext> SetupRegistry()
        {
            var registry = new TransitionRegistry<TicketState, TicketContext>();
            registry.RegisterCondition("HasAgent", ctx => ctx.IsAgentAssigned);
            registry.RegisterCondition("Confirmed", ctx => ctx.IsCustomerConfirmed);
            registry.RegisterSideEffect("NotifyCustomer", (ctx, _, _) => ctx.IsCustomerConfirmed = true);
            return registry;
        }

        private static EnumStateMachineDefinition<TicketState, TicketContext> BuildDefinition(TransitionRegistry<TicketState, TicketContext> registry)
        {
            var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .WithInitialState(TicketState.Open)
                .AddTransition(TicketState.Open, TicketState.InProgress)
                    .When(registry.Conditions["HasAgent"], "HasAgent")
                    .WithSideEffect(registry.SideEffects["NotifyCustomer"], "NotifyCustomer")
                    .Done()
                .AddTransition(TicketState.InProgress, TicketState.Resolved)
                    .When(registry.Conditions["Confirmed"], "Confirmed")
                    .Done();

            return builder.Build();
        }

        [Fact]
        public void Test_ValidTransition_Works()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);
            var context = new TicketContext { IsAgentAssigned = true };

            Assert.True(sm.TryTransitionTo(TicketState.InProgress, context));
            Assert.Equal(TicketState.InProgress, sm.Current);
        }

        [Fact]
        public void Test_InvalidTransition_Fails()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);
            var context = new TicketContext { IsAgentAssigned = false };

            // Cannot transition to InProgress without an agent
            Assert.False(sm.TryTransitionTo(TicketState.InProgress, context));

            // Cannot transition to Resolved directly
            Assert.False(sm.TryTransitionTo(TicketState.Resolved, context));

            Assert.Equal(TicketState.Open, sm.Current);
        }

        [Fact]
        public void Test_SerializationToJsonAndBack_Works()
        {
            var registry = SetupRegistry();
            var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .WithInitialState(TicketState.Open)
                .AddTransition(TicketState.Open, TicketState.InProgress)
                    .When(registry.Conditions["HasAgent"], "HasAgent")
                    .WithSideEffect(registry.SideEffects["NotifyCustomer"], "NotifyCustomer")
                    .Done();

            var dto = builder.ToSerializable();
            string json = JsonSerializer.Serialize(dto);
            var deserializedDto = JsonSerializer.Deserialize<SerializableStateMachine>(json);

            var loadedBuilder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .LoadFrom(deserializedDto!, registry);

            var sm = new FiniteStateMachine<TicketState, TicketContext>(loadedBuilder.Build());
            var context = new TicketContext { IsAgentAssigned = true };

            Assert.True(sm.TryTransitionTo(TicketState.InProgress, context));
            Assert.Equal(TicketState.InProgress, sm.Current);
        }

        [Fact]
        public void Test_SerializationToYamlAndBack_Works()
        {
            var registry = SetupRegistry();
            var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .WithInitialState(TicketState.Open)
                .AddTransition(TicketState.Open, TicketState.InProgress)
                    .When(registry.Conditions["HasAgent"], "HasAgent")
                    .WithSideEffect(registry.SideEffects["NotifyCustomer"], "NotifyCustomer")
                    .Done();

            var dto = builder.ToSerializable();

            var yamlSerializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yamlDeserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build();

            var yaml = yamlSerializer.Serialize(dto);

            var deserializedDto = yamlDeserializer.Deserialize<SerializableStateMachine>(new StringReader(yaml));

            var loadedBuilder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .LoadFrom(deserializedDto!, registry);

            var sm = new FiniteStateMachine<TicketState, TicketContext>(loadedBuilder.Build());
            var context = new TicketContext { IsAgentAssigned = true };

            Assert.True(sm.TryTransitionTo(TicketState.InProgress, context));
            Assert.Equal(TicketState.InProgress, sm.Current);
        }

        [Fact]
        public void Test_InvalidEnum_ThrowsOnLoad()
        {
            var dto = new SerializableStateMachine
            {
                EntityType = "Ticket",
                InitialState = "NonExistent",
                States = new List<string> { "Open", "InProgress" },
                Transitions = new List<SerializableTransition>()
            };
            var registry = SetupRegistry();

            Assert.Throws<ArgumentException>(() =>
                FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                    .LoadFrom(dto, registry));
        }

        [Fact]
        public void Test_CircularTransition_Success()
        {
            var registry = new TransitionRegistry<TicketState, TicketContext>();
            registry.RegisterCondition("Always", _ => true);

            var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .WithInitialState(TicketState.Open)
                .AddTransition(TicketState.Open, TicketState.InProgress)
                    .When(registry.Conditions["Always"], "Always")
                    .Done()
                .AddTransition(TicketState.InProgress, TicketState.Open)
                    .When(registry.Conditions["Always"], "Always")
                    .Done();

            var sm = new FiniteStateMachine<TicketState, TicketContext>(builder.Build());

            Assert.True(sm.TryTransitionTo(TicketState.InProgress, new TicketContext()));
            Assert.True(sm.TryTransitionTo(TicketState.Open, new TicketContext()));
        }

        [Fact]
        public void Test_MissingSideEffect_NoException()
        {
            var registry = new TransitionRegistry<TicketState, TicketContext>();
            registry.RegisterCondition("Always", _ => true);
            // Side effect is missing on purpose

            var dto = new SerializableStateMachine
            {
                EntityType = "Ticket",
                InitialState = "Open",
                States = new List<string> { "Open", "InProgress" },
                Transitions = new List<SerializableTransition>
                {
                    new SerializableTransition
                    {
                        From = "Open",
                        To = "InProgress",
                        ConditionName = "Always",
                        SideEffectName = "MissingEffect"
                    }
                }
            };

            var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
                .LoadFrom(dto, registry);

            var sm = new FiniteStateMachine<TicketState, TicketContext>(builder.Build());
            Assert.True(sm.TryTransitionTo(TicketState.InProgress, new TicketContext()));
        }

        [Fact]
        public void Test_RepeatedTransitionToSameState_Fails()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);
            var context = new TicketContext { IsAgentAssigned = true };

            Assert.True(sm.TryTransitionTo(TicketState.InProgress, context));
            // Already in InProgress, should not transition again
            Assert.False(sm.TryTransitionTo(TicketState.InProgress, context));
            Assert.Equal(TicketState.InProgress, sm.Current);
        }

        [Fact]
        public void Test_SideEffect_IsExecuted()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);
            var context = new TicketContext { IsAgentAssigned = true, IsCustomerConfirmed = false };

            Assert.False(context.IsCustomerConfirmed);
            Assert.True(sm.TryTransitionTo(TicketState.InProgress, context));
            // Side effect should set IsCustomerConfirmed to true
            Assert.True(context.IsCustomerConfirmed);
        }

        [Fact]
        public void Test_NullContext_ThrowsArgumentNullException()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);

            Assert.Throws<ArgumentNullException>(() => sm.TryTransitionTo(TicketState.InProgress, null!));
        }

        [Fact]
        public void Test_TransitionToInvalidState_Fails()
        {
            var registry = SetupRegistry();
            var definition = BuildDefinition(registry);
            var sm = new FiniteStateMachine<TicketState, TicketContext>(definition);
            var context = new TicketContext { IsAgentAssigned = true };

            // "Closed" is not a valid target from "Open" or "InProgress" in this definition
            Assert.False(sm.TryTransitionTo(TicketState.Closed, context));
            Assert.Equal(TicketState.Open, sm.Current);
        }

    }

    
}