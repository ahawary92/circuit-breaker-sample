namespace CircuitBreaker.Core.Interfaces.Handlers
{
    public interface ICheckoutHandler
    {
        string RequestCheckout(bool withError);
    }
}
