using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFsm.Example
{
    public class MooreMachine
    {
        public enum MooreState { S0, S1 }
        public class MooreContext
        {
            public string Output { get; set; }
        }

        public MooreMachine()
        {
            var registry = new TransitionRegistry<MooreState, MooreContext>();
            registry.RegisterSideEffect("OutputSideEffect", (MooreContext ctx, MooreState oldState, MooreState newState) => {
                switch (newState) 
                {
                    case MooreState.S0: ctx.Output = "A"; break;
                    case MooreState.S1: ctx.Output = "B"; break;
                    default: ctx.Output = "C"; break;
                }
            });

            var builder = FiniteStateMachineBuilder<MooreState, MooreContext>.Create("Moore")
                .WithInitialState(MooreState.S0)
                .WithRegistry(registry)
                .AddTransition(MooreState.S0, MooreState.S1)
                    .WithSideEffect("OutputSideEffect")
                    .Done()
                .AddTransition(MooreState.S1, MooreState.S0)
                    .WithSideEffect("OutputSideEffect")
                    .Done();

            var fsm = new FiniteStateMachine<MooreState, MooreContext>(builder.Build());
        }
    }
}
