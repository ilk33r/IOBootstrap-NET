using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common;

[Serializable]
public class IOInvalidNonceException : IOServiceException
{
    public IOInvalidNonceException() : base(IOExceptionMessages.InvalidNonceCode, IOExceptionMessages.InvalidNonceMessage, "")
    {
    }

    public IOInvalidNonceException(string detailedMessage) : base(IOExceptionMessages.InvalidNonceCode, IOExceptionMessages.InvalidNonceMessage, detailedMessage)
    {
    }
}

