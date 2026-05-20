using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Files;

[Serializable]
public class IOFileNotFoundException : IOServiceException
{
    public IOFileNotFoundException() : base(IOExceptionMessages.FileNotFoundCode, IOExceptionMessages.FileNotFoundMessage, "")
    {
    }

    public IOFileNotFoundException(string detailedMessage) : base(IOExceptionMessages.FileNotFoundCode, IOExceptionMessages.FileNotFoundMessage, detailedMessage)
    {
    }
}
