using CircuitBreaker.Core.Interfaces.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreaker.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForgetPasswordController : ControllerBase
    {
        private readonly IForgetPasswordHandler forgetPasswordHandler;
        public ForgetPasswordController(IForgetPasswordHandler fph)
        {
            forgetPasswordHandler = fph;
        }

        [HttpGet(Name = "ForgetPassword")]
        public string Get()
        {
            string result = forgetPasswordHandler.RequestForgetPassword();

            return result;
        }
    }
}
