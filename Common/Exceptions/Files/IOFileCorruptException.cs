using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Files;

[Serializable]
public class IOFileCorruptException : IOServiceException
{
    public IOFileCorruptException() : base(IOExceptionMessages.FileCorruptCode, IOExceptionMessages.FileCorruptMessage, "")
    {
    }

    public IOFileCorruptException(string detailedMessage) : base(IOExceptionMessages.FileCorruptCode, IOExceptionMessages.FileCorruptMessage, detailedMessage)
    {
    }
}
