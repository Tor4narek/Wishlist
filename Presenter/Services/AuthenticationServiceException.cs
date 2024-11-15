using System;

namespace Presenter.Services
{
    public class AuthenticationServiceException : Exception
    {
        public AuthenticationServiceException() : base() { }

        public AuthenticationServiceException(string message) : base(message) { }

        public AuthenticationServiceException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}