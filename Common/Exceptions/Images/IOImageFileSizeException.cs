using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Images;

[Serializable]
public class IOImageFileSizeException : IOServiceException
{
    public IOImageFileSizeException() : base(IOExceptionMessages.ImageFileSizeCode, IOExceptionMessages.ImageFileSizeMessage, "")
    {
    }

    public IOImageFileSizeException(string detailedMessage) : base(IOExceptionMessages.ImageFileSizeCode, IOExceptionMessages.ImageFileSizeMessage, detailedMessage)
    {
    }
}