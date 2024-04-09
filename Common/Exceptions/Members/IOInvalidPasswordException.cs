using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members;

[Serializable]
public class IOInvalidPasswordException : IOServiceException
{
    public IOInvalidPasswordException() : base(IOExceptionMessages.InvalidPasswordsCode, IOExceptionMessages.InvalidPasswordsMessage, "")
    {
    }

    public IOInvalidPasswordException(string detailedMessage) : base(IOExceptionMessages.InvalidPasswordsCode, IOExceptionMessages.InvalidPasswordsMessage, detailedMessage)
    {
    }
}

