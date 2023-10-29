using CircuitBreaker.Core.Interfaces.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker.Service.Handlers
{
    public class EmailHandler : IEmailHandler
    {
        public void SendEmail()
        {
            Console.WriteLine("Password Reset Email Sent");
        }
    }
}
