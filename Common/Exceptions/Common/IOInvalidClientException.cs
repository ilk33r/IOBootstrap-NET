using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOInvalidClientException : IOServiceException
    {
        public IOInvalidClientException() : base(IOExceptionMessages.InvalidClientCode, IOExceptionMessages.InvalidClientMessage, "")
        {
        }

        public IOInvalidClientException(string detailedMessage) : base(IOExceptionMessages.InvalidClientCode, IOExceptionMessages.InvalidClientMessage, detailedMessage)
        {
        }
    }
}
