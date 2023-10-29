using CircuitBreaker.Core.Exceptions;
using CircuitBreaker.Core.Interfaces.Handlers;
using CircuitBreaker.Core.Interfaces.States;
using CircuitBreaker.Core.States;
using CircuitBreaker.Service.Queues;

namespace CircuitBreaker.Service.Handlers
{
    public class ForgetPasswordHandler : IForgetPasswordHandler
    {
        private readonly IHalfOpenState halfOpenState;
        private readonly IEmailHandler emailHandler;
        private readonly int maxQueueSize = 10;

        public ForgetPasswordHandler(IHalfOpenState hos, IEmailHandler eh)
        {
            halfOpenState = hos;
            emailHandler = eh;
        }

        public string RequestForgetPassword()
        {
            string status = "OK";
            try
            {
                if (RateLimiter.CanRequest())
                {
                    Core.CircuitBreaker.Execute(() =>
                    {
                        emailHandler.SendEmail();
                    });
                    status = "Password Reset Email Sent";
                }
                else
                {
                    status = "Request throttled. Please try again later.";
                }
            }
            catch (CircuitOpenException ex)
            {
                status = ex.Message;

                // If the circuit is open, enforce request throttling
                if (ForgotPasswordQueue.Count() < maxQueueSize)
                {
                    var currentTime = DateTime.Now;
                    var waitTime = TimeSpan.FromMinutes(1); // Example wait time for throttled requests
                    var nextRequestTime = currentTime.Add(waitTime);

                    ForgotPasswordQueue.EnqueueRequest(() =>
                    {
                        Console.WriteLine($"Password Reset Email Throttled. Please try again after {nextRequestTime}");
                    });
                }
                else
                {
                    status += ", Forgot Password Queue is full.";
                }
            }

            // Transition to the Half-Open state and allow some throttled requests to go through

            if (ForgotPasswordQueue.Count() >= 5)
            {
                Core.CircuitBreaker.TransitionToState(halfOpenState);
                int requestsProcessedInHalfOpenState = 0;

                while (requestsProcessedInHalfOpenState < maxQueueSize && ForgotPasswordQueue.Count() > 0)
                {
                    if (ForgotPasswordQueue.TryDequeueRequest(out var throttledRequest))
                    {
                        Core.CircuitBreaker.Execute(throttledRequest);
                        requestsProcessedInHalfOpenState++;
                    }
                }

                // If the email service can handle more requests, transition back to the Closed state
                if (requestsProcessedInHalfOpenState == maxQueueSize)
                {
                    Core.CircuitBreaker.TransitionToState(new ClosedState());
                }

                // Process any remaining throttled requests
                while (ForgotPasswordQueue.TryDequeueRequest(out var throttledRequest))
                {
                    Core.CircuitBreaker.Execute(throttledRequest);
                }
            }
      
            return status;
        }
    }
}
