using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Members;

[Serializable]
public class IOUserDeactivatedException : IOServiceException
{
    public IOUserDeactivatedException() : base(IOExceptionMessages.UserDeactivatedCode, IOExceptionMessages.UserDeactivatedMessage, "")
    {
    }

    public IOUserDeactivatedException(string detailedMessage) : base(IOExceptionMessages.UserDeactivatedCode, IOExceptionMessages.UserDeactivatedMessage, detailedMessage)
    {
    }
}