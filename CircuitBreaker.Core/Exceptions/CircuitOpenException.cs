
namespace CircuitBreaker.Core.Exceptions
{
    public class CircuitOpenException : Exception
    {
        public CircuitOpenException(string message) : base(message) { }
    }
}
