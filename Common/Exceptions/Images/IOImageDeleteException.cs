using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Images
{
    [Serializable]
    public class IOImageDeleteException : IOServiceException
    {
        public IOImageDeleteException() : base(IOExceptionMessages.ImageDeleteCode, IOExceptionMessages.ImageDeleteMessage, "")
        {
        }

        public IOImageDeleteException(string detailedMessage) : base(IOExceptionMessages.ImageDeleteCode, IOExceptionMessages.ImageDeleteMessage, detailedMessage)
        {
        }
    }
}
