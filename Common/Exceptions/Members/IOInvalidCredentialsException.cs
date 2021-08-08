using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members
{
    [Serializable]
    public class IOInvalidCredentialsException : IOServiceException
    {
        public IOInvalidCredentialsException() : base(IOExceptionMessages.InvalidCredentialsCode, IOExceptionMessages.InvalidCredentialsMessage, "")
        {
        }

        public IOInvalidCredentialsException(string detailedMessage) : base(IOExceptionMessages.InvalidCredentialsCode, IOExceptionMessages.InvalidCredentialsMessage, detailedMessage)
        {
        }
    }
}
