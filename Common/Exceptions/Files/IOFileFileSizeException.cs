using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Files;

[Serializable]
public class IOFileFileSizeException : IOServiceException
{
    public IOFileFileSizeException() : base(IOExceptionMessages.FileFileSizeCode, IOExceptionMessages.FileFileSizeMessage, "")
    {
    }

    public IOFileFileSizeException(string detailedMessage) : base(IOExceptionMessages.FileFileSizeCode, IOExceptionMessages.FileFileSizeMessage, detailedMessage)
    {
    }
}
