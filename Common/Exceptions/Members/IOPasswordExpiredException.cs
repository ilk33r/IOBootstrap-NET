using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members;

[Serializable]
public class IOPasswordExpiredException : IOServiceException
{
    public IOPasswordExpiredException() : base(IOExceptionMessages.PasswordExpiredCode, IOExceptionMessages.PasswordExpiredMessage, "")
    {
    }

    public IOPasswordExpiredException(string detailedMessage) : base(IOExceptionMessages.PasswordExpiredCode, IOExceptionMessages.PasswordExpiredMessage, detailedMessage)
    {
    }
}
