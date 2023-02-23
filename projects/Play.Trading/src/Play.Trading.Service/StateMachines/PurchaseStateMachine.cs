using Automatonymous;

namespace Play.Trading.Service.StateMachines
{
    public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
    {
        public State Accepted { get; set; }
        public State ItemsGranted { get; set; }
        public State Completed { get; set; }
        public State Faulted { get; set; }

        public PurchaseStateMachine()
        {
            InstanceState(state => state.CurrentState);
        }
    }
}
