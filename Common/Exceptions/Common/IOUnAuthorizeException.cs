using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOUnAuthorizeException : IOServiceException
    {
        public IOUnAuthorizeException() : base(IOExceptionMessages.UnauthorizedCode, IOExceptionMessages.UnauthorizedMessage, "")
        {
        }

        public IOUnAuthorizeException(string detailedMessage) : base(IOExceptionMessages.UnauthorizedCode, IOExceptionMessages.UnauthorizedMessage, detailedMessage)
        {
        }
    }
}
