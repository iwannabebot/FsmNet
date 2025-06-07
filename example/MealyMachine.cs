using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFsm.Example
{
    public class MealyMachine
    {
        public enum MealyState { S0, S1 }
        public class MealyContext
        {
            public string Input { get; set; }
            public string Output { get; set; }
        }

        public MealyMachine()
        {

            var registry = new TransitionRegistry<MealyState, MealyContext>();
            registry.RegisterSideEffect("OutputA", (ctx, _, _) => ctx.Output = "A");
            registry.RegisterSideEffect("OutputB", (ctx, _, _) => ctx.Output = "B");

            var builder = FiniteStateMachineBuilder<MealyState, MealyContext>.Create("Mealy")
                .WithInitialState(MealyState.S0)
                .WithRegistry(registry)
                .AddTransition(MealyState.S0, MealyState.S1)
                    .When(ctx => ctx.Input == "x")
                    .WithSideEffect("OutputA")
                    .Done()
                .AddTransition(MealyState.S1, MealyState.S0)
                    .When(ctx => ctx.Input == "y")
                    .WithSideEffect("OutputB")
                    .Done();
            var fsm = new FiniteStateMachine<MealyState, MealyContext>(builder.Build());
        }
    }
}
