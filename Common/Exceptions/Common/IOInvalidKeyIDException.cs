using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOInvalidKeyIDException : IOServiceException
    {
        public IOInvalidKeyIDException() : base(IOExceptionMessages.InvalidKeyIDCode, IOExceptionMessages.InvalidKeyIDMessage, "")
        {
        }

        public IOInvalidKeyIDException(string detailedMessage) : base(IOExceptionMessages.InvalidKeyIDCode, IOExceptionMessages.InvalidKeyIDMessage, detailedMessage)
        {
        }
    }
}
