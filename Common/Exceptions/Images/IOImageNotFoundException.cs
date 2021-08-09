using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Images
{
    [Serializable]
    public class IOImageNotFoundException : IOServiceException
    {
        public IOImageNotFoundException() : base(IOExceptionMessages.ImageNotFoundCode, IOExceptionMessages.ImageNotFoundMessage, "")
        {
        }

        public IOImageNotFoundException(string detailedMessage) : base(IOExceptionMessages.ImageNotFoundCode, IOExceptionMessages.ImageNotFoundMessage, detailedMessage)
        {
        }
    }
}
