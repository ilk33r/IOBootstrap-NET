using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;

namespace IOBootstrap.Net.Common.Exceptions.Common
{
    public class IOMWConnectionException : IOServiceException
    {
        public IOMWConnectionException() : base(IOExceptionMessages.MWConnectionCode, IOExceptionMessages.MWConnectionMessage, "")
        {
        }

        public IOMWConnectionException(string detailedMessage) : base(IOExceptionMessages.MWConnectionCode, IOExceptionMessages.MWConnectionMessage, detailedMessage)
        {
        }
    }
}
