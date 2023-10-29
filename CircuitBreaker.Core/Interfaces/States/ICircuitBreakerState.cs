namespace CircuitBreaker.Core.Interfaces.States
{
    public interface ICircuitBreakerState
    {
        void ExecuteAction(Action action);
    }
}
