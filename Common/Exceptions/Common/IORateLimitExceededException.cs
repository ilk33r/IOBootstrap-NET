using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common;

[Serializable]
public class IORateLimitExceededException : IOServiceException
{
    public IORateLimitExceededException() : base(IOExceptionMessages.RateLimitExceededCode, IOExceptionMessages.RateLimitExceededMessage, "")
    {
    }

    public IORateLimitExceededException(string detailedMessage) : base(IOExceptionMessages.RateLimitExceededCode, IOExceptionMessages.RateLimitExceededMessage, detailedMessage)
    {
    }
}