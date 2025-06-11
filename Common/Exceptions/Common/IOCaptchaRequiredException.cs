using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common;

[Serializable]
public class IOCaptchaRequiredException : IOServiceException
{
    public IOCaptchaRequiredException() : base(IOExceptionMessages.CaptchaRequiredCode, IOExceptionMessages.CaptchaRequiredMessage, "")
    {
    }

    public IOCaptchaRequiredException(string detailedMessage) : base(IOExceptionMessages.CaptchaRequiredCode, IOExceptionMessages.CaptchaRequiredMessage, detailedMessage)
    {
    }
}