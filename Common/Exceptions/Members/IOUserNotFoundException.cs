using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members
{
    [Serializable]
    public class IOUserNotFoundException : IOServiceException
    {
        public IOUserNotFoundException() : base(IOExceptionMessages.UserNotFoundCode, IOExceptionMessages.UserNotFoundMessage, "")
        {
        }

        public IOUserNotFoundException(string detailedMessage) : base(IOExceptionMessages.UserNotFoundCode, IOExceptionMessages.UserNotFoundMessage, detailedMessage)
        {
        }
    }
}
