using CircuitBreaker.Core.Interfaces.States;

namespace CircuitBreaker.Core.States
{
    public class HalfOpenState : IHalfOpenState
    {
        private static int testFailureCount;
        private static int testThreshold = 2; // Number of test requests before transitioning back
        private readonly IOpenState openState;
        private readonly IClosedState closedState;

        public HalfOpenState(IOpenState os, IClosedState cs)
        {
            openState = os;
            closedState = cs;
        }

        public void ExecuteAction(Action action)
        {
            try
            {
                action.Invoke(); // Execute the action

                // If the test request succeeds, reset the test failure count
                testFailureCount = 0;

                // Transition back to the Closed state if the test succeeded
                CircuitBreaker.TransitionToState(closedState);
            }
            catch (Exception)
            {
                // If the test request fails, increment the test failure count
                testFailureCount++;

                // If the test failure count exceeds the threshold, transition back to the Open state
                if (testFailureCount >= testThreshold)
                {
                    CircuitBreaker.TransitionToState(openState);
                }
            }
        }
    }
}
