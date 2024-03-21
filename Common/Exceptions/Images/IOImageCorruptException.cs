using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Images;

[Serializable]
public class IOImageCorruptException : IOServiceException
{
    public IOImageCorruptException() : base(IOExceptionMessages.ImageCorruptCode, IOExceptionMessages.ImageCorruptMessage, "")
    {
    }

    public IOImageCorruptException(string detailedMessage) : base(IOExceptionMessages.ImageCorruptCode, IOExceptionMessages.ImageCorruptMessage, detailedMessage)
    {
    }
}