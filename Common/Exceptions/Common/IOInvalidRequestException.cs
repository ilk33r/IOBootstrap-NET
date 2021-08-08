using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOInvalidRequestException : IOServiceException
    {
        public IOInvalidRequestException() : base(IOExceptionMessages.InvalidRequestCode, IOExceptionMessages.InvalidRequestMessage, "")
        {
        }

        public IOInvalidRequestException(string detailedMessage) : base(IOExceptionMessages.InvalidRequestCode, IOExceptionMessages.InvalidRequestMessage, detailedMessage)
        {
        }
    }
}
