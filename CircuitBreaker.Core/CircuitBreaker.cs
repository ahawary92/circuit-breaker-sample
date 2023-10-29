using CircuitBreaker.Core.Interfaces.States;
using CircuitBreaker.Core.States;

namespace CircuitBreaker.Core
{
    public static class CircuitBreaker
    {
        private static ICircuitBreakerState state;
        private static int failureCount;
        private static int failureThreshold = 2;

        static CircuitBreaker()
        {
            state = new ClosedState();
        }

        public static void Execute(Action action)
        {
            state.ExecuteAction(action);
        }

        public static void IncrementFailureCount()
        {
            failureCount++;
        }

        public static void ResetFailureCount()
        {
            failureCount = 0;
        }

        public static bool IsThresholdReached()
        {
            return failureCount >= failureThreshold;
        }

        public static void TransitionToState(ICircuitBreakerState newState)
        {
            state = newState;
        }
    }
}