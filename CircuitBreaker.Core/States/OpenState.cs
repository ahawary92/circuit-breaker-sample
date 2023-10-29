using CircuitBreaker.Core.Exceptions;
using CircuitBreaker.Core.Interfaces.States;

namespace CircuitBreaker.Core.States
{
    public class OpenState : IOpenState
    {
        public void ExecuteAction(Action action)
        {
            throw new CircuitOpenException("Circuit is open. Requests are blocked.");
        }
    }
}
