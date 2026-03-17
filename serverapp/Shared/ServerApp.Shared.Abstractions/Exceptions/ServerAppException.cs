using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Exceptions
{
    public class ServerAppException : Exception
    {
        protected ServerAppException(string message) : base(message)
        {

        }
    }
}
