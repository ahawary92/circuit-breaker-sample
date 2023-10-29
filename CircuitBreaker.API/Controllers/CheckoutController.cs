using CircuitBreaker.Core.Interfaces.Handlers;
using CircuitBreaker.Service;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreaker.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutHandler checkoutHandler;
        public CheckoutController(ICheckoutHandler ch)
        {
            checkoutHandler = ch;
        }

        [HttpGet(Name = "Checkout")]
        public string Get(bool withError)
        {
            string result = checkoutHandler.RequestCheckout(withError);

            return result;
        }
    }
}
