using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members
{
    [Serializable]
    public class IOUserExistsException : IOServiceException
    {
        public IOUserExistsException() : base(IOExceptionMessages.UserExistsCode, IOExceptionMessages.UserExistsMessage, "")
        {
        }

        public IOUserExistsException(string detailedMessage) : base(IOExceptionMessages.UserExistsCode, IOExceptionMessages.UserExistsMessage, detailedMessage)
        {
        }
    }
}
