using CircuitBreaker.Core.Exceptions;
using CircuitBreaker.Core.Interfaces.Handlers;
using CircuitBreaker.Core.Interfaces.States;
using CircuitBreaker.Service.Queues;

namespace CircuitBreaker.Service.Handlers
{
    public class CheckoutHandler : ICheckoutHandler
    {
        private readonly IHalfOpenState halfOpenState;
        private readonly IClosedState closedState;
        private readonly IPaymentHandler paymentHandler;
        private readonly int maxQueueSize = 10;

        public CheckoutHandler(IHalfOpenState hos, IClosedState cs, IPaymentHandler ph)
        {
            halfOpenState = hos;
            closedState = cs;
            paymentHandler = ph;
        }

        public string RequestCheckout(bool withError)
        {
            string status = "OK";

            try
            {
                if (withError)
                {
                    Core.CircuitBreaker.Execute(() =>
                    {
                        throw new Exception("Something went wrong");
                    });
                    status = "Something went wrong";
                }
                else
                {
                    Core.CircuitBreaker.Execute(() =>
                    {
                        paymentHandler.ProcessPayment();
                    });
                    status = "Payment processed";
                }
            }
            catch (CircuitOpenException ex)
            {
                status = ex.Message;
                // If the circuit is open, enqueue the payment request
                if (CheckoutQueue.Count() < maxQueueSize)
                {
                    CheckoutQueue.EnqueueRequest(() =>
                    {
                        Console.WriteLine("Payment Enqueued");
                    });
                }
                else
                {
                   status = "Checkout Queue is full.";
                }
            }

            if (CheckoutQueue.Count() >= 5)
            {
                // Transition to the Half-Open state and allow some queued requests to go through
                Core.CircuitBreaker.TransitionToState(halfOpenState);
                int requestsProcessedInHalfOpenState = 0;

                while (requestsProcessedInHalfOpenState < maxQueueSize && CheckoutQueue.Count() > 0)
                {
                    if (CheckoutQueue.TryDequeueRequest(out var queuedRequest))
                    {
                        try
                        {
                            Core.CircuitBreaker.Execute(queuedRequest);
                            requestsProcessedInHalfOpenState++;
                        }
                        catch (CircuitOpenException)
                        {
                            // If the circuit is open, re-enqueue the request
                            CheckoutQueue.EnqueueRequest(queuedRequest);
                        }
                    }
                }

                // If the payment gateway appears to be stable, transition back to the Closed state
                if (requestsProcessedInHalfOpenState == maxQueueSize)
                {
                    Core.CircuitBreaker.TransitionToState(closedState);
                }

                // Process any remaining queued requests
                while (CheckoutQueue.TryDequeueRequest(out var queuedRequest))
                {
                    Core.CircuitBreaker.Execute(queuedRequest);
                }
            }

            return status;
        }
    }
}
