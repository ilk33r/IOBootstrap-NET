using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.NET.Common.Exceptions.Common
{
    [Serializable]
    public class IOHttpsRequiredException : IOServiceException
    {
        public IOHttpsRequiredException() : base(IOExceptionMessages.HttpsRequiredCode, IOExceptionMessages.HttpsRequiredMessage, "")
        {
        }

        public IOHttpsRequiredException(string detailedMessage) : base(IOExceptionMessages.HttpsRequiredCode, IOExceptionMessages.HttpsRequiredMessage, detailedMessage)
        {
        }
    }
}
