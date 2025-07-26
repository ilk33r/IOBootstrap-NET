using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common;

[Serializable]
public class IOInvalidAPIException : IOServiceException
{
    public IOInvalidAPIException() : base(IOExceptionMessages.InvalidAPICode, IOExceptionMessages.InvalidAPIMessage, "")
    {
    }

    public IOInvalidAPIException(string detailedMessage) : base(IOExceptionMessages.InvalidAPICode, IOExceptionMessages.InvalidAPIMessage, detailedMessage)
    {
    }
}
