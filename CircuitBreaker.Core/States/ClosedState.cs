using CircuitBreaker.Core.Interfaces.States;

namespace CircuitBreaker.Core.States
{
    public class ClosedState : IClosedState
    {
        public void ExecuteAction(Action action)
        {
            try
            {
                action.Invoke();
                CircuitBreaker.ResetFailureCount();
            }
            catch (Exception)
            {
                CircuitBreaker.IncrementFailureCount();

                if (CircuitBreaker.IsThresholdReached())
                {
                    CircuitBreaker.TransitionToState(new OpenState());
                }
            }
        }
    }
}
