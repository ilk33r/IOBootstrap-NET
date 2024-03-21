using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Images;

[Serializable]
public class IOImageSaveException : IOServiceException
{
    public IOImageSaveException() : base(IOExceptionMessages.ImageSaveCode, IOExceptionMessages.ImageSaveMessage, "")
    {
    }

    public IOImageSaveException(string detailedMessage) : base(IOExceptionMessages.ImageSaveCode, IOExceptionMessages.ImageSaveMessage, detailedMessage)
    {
    }
}