using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Files;

[Serializable]
public class IOFileSaveException : IOServiceException
{
    public IOFileSaveException() : base(IOExceptionMessages.FileSaveCode, IOExceptionMessages.FileSaveMessage, "")
    {
    }

    public IOFileSaveException(string detailedMessage) : base(IOExceptionMessages.FileSaveCode, IOExceptionMessages.FileSaveMessage, detailedMessage)
    {
    }
}
