using SharpFsm;
using SharpFsm.Serialization;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    static void Main()
    {
        var registry = new TransitionRegistry<OrderContext>();
        registry.RegisterCondition("PaymentReceived", ctx => ctx.PaymentReceived);
        registry.RegisterCondition("PackingComplete", ctx => ctx.PackingComplete);
        registry.RegisterCondition("Shipped", ctx => ctx.Shipped);
        registry.RegisterCondition("Delivered", ctx => ctx.Delivered);
        registry.RegisterCondition("CancelRequested", ctx => ctx.CancelRequested);
        registry.RegisterCondition("ReturnRequested", ctx => ctx.ReturnRequested);

        registry.RegisterSideEffect("NotifyShipment", ctx => Console.WriteLine("Customer notified: Order shipped"));
        registry.RegisterSideEffect("NotifyDelivery", ctx => Console.WriteLine("Customer notified: Order delivered"));
        registry.RegisterSideEffect("NotifyCancel", ctx => Console.WriteLine("Customer notified: Order cancelled"));
        registry.RegisterSideEffect("NotifyReturn", ctx => Console.WriteLine("Customer notified: Order returned"));

        var builder = FiniteStateMachineBuilder<OrderState, OrderContext>.Create("Order")
            .WithInitialState(OrderState.Created)
            .WithRegistry(registry)
            .AddTransition(OrderState.Created, OrderState.Paid)
                .When("PaymentReceived")
                .Done()
            .AddTransition(OrderState.Paid, OrderState.Packed)
                .When("PackingComplete")
                .Done()
            .AddTransition(OrderState.Packed, OrderState.Shipped)
                .When("Shipped")
                .WithSideEffect("NotifyShipment")
                .Done()
            .AddTransition(OrderState.Shipped, OrderState.Delivered)
                .When("Delivered")
                .WithSideEffect("NotifyDelivery")
                .Done()
            .AddTransition(OrderState.Created, OrderState.Cancelled)
                .When("CancelRequested")
                .WithSideEffect("NotifyCancel")
                .Done()
            .AddTransition(OrderState.Paid, OrderState.Cancelled)
                .When("CancelRequested")
                .WithSideEffect("NotifyCancel")
                .Done()
            .AddTransition(OrderState.Packed, OrderState.Cancelled)
                .When("CancelRequested")
                .WithSideEffect("NotifyCancel")
                .Done()
            .AddTransition(OrderState.Shipped, OrderState.Cancelled)
                .When("CancelRequested")
                .WithSideEffect("NotifyCancel")
                .Done()
            .AddTransition(OrderState.Delivered, OrderState.Returned)
                .When("ReturnRequested")
                .WithSideEffect("NotifyReturn")
                .Done();

        var fsm = new FiniteStateMachine<OrderState, OrderContext>(builder.Build());

        var context = new OrderContext { PaymentReceived = true };
        fsm.TryTransitionTo(OrderState.Paid, context); // Moves to Paid

        context.PackingComplete = true;
        fsm.TryTransitionTo(OrderState.Packed, context); // Moves to Packed

        context.Shipped = true;
        fsm.TryTransitionTo(OrderState.Shipped, context); // Moves to Shipped, notifies shipment

        context.Delivered = true;
        fsm.TryTransitionTo(OrderState.Delivered, context); // Moves to Delivered, notifies delivery

        context.ReturnRequested = true;
        fsm.TryTransitionTo(OrderState.Returned, context); // Moves to Returned, notifies return
    }
}

public enum OrderState
{
    Created,
    Paid,
    Packed,
    Shipped,
    Delivered,
    Cancelled,
    Returned
}

public class OrderContext
{
    public bool PaymentReceived { get; set; }
    public bool PackingComplete { get; set; }
    public bool Shipped { get; set; }
    public bool Delivered { get; set; }
    public bool CancelRequested { get; set; }
    public bool ReturnRequested { get; set; }
}