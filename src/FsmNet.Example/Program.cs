using FsmNet;
using FsmNet.Serialization;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    static void Main()
    {
        var registry = new TransitionRegistry<TicketContext>();
        registry.RegisterCondition("HasAgent", ctx => ctx.IsAgentAssigned);
        registry.RegisterCondition("Confirmed", ctx => ctx.IsCustomerConfirmed);
        registry.RegisterSideEffect("NotifyCustomer", ctx => Console.WriteLine("Customer notified"));

        var builder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create("Ticket")
            .WithInitialState(TicketState.Open)
            .AddTransition(TicketState.Open, TicketState.InProgress)
                .When(registry.Conditions["HasAgent"], "HasAgent")
                .WithSideEffect(registry.SideEffects["NotifyCustomer"], "NotifyCustomer")
                .Done()
            .AddTransition(TicketState.InProgress, TicketState.Resolved)
                .When(registry.Conditions["Confirmed"], "Confirmed")
                .Done();

        var definition = builder.Build();

        var serializable = builder.ToSerializable();

        // JSON using System.Text.Json
        string json = JsonSerializer.Serialize(serializable, new JsonSerializerOptions { WriteIndented = true });

        // YAML using YamlDotNet
        var yamlSerializer = new YamlDotNet.Serialization.Serializer();
        string yaml = yamlSerializer.Serialize(serializable);

        // From JSON
        var jsonDto = JsonSerializer.Deserialize<SerializableStateMachine>(json)!;

        // From YAML
        var yamlDeserializer = new YamlDotNet.Serialization.Deserializer();
        var yamlDto = yamlDeserializer.Deserialize<SerializableStateMachine>(new StringReader(yaml));

        // Rebuild FSM
        var loadedBuilder = FiniteStateMachineBuilder<TicketState, TicketContext>.Create(jsonDto.EntityType)
            .LoadFrom(jsonDto, registry);

        var loadedFsm = loadedBuilder.Build();

        // Runtime FSM Usage
        var sm = new FiniteStateMachine<TicketState, TicketContext>(loadedFsm);
        var context = new TicketContext { IsAgentAssigned = true, IsCustomerConfirmed = false };

        Console.WriteLine($"Current: {sm.Current}"); // Open

        if (sm.TryTransitionTo(TicketState.InProgress, context))
            Console.WriteLine($"Now: {sm.Current}"); // InProgress


    }
}

public enum TicketState
{
    Open,
    InProgress,
    Resolved,
    Closed
}

public class TicketContext
{
    public bool IsAgentAssigned { get; set; }
    public bool IsCustomerConfirmed { get; set; }
}