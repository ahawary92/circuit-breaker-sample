using CircuitBreaker.Core.Interfaces.Handlers;

namespace CircuitBreaker.Service.Handlers
{
    public class PaymentHandler : IPaymentHandler
    {
        public void ProcessPayment()
        {
            Console.WriteLine("Payment Success");
        }
    }
}
