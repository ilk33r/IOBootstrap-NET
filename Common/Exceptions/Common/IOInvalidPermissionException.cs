using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOInvalidPermissionException : IOServiceException
    {
        public IOInvalidPermissionException() : base(IOExceptionMessages.InvalidPermissionCode, IOExceptionMessages.InvalidPermissionMessage, "")
        {
        }

        public IOInvalidPermissionException(string detailedMessage) : base(IOExceptionMessages.InvalidPermissionCode, IOExceptionMessages.InvalidPermissionMessage, detailedMessage)
        {
        }
    }
}
