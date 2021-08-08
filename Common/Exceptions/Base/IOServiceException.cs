using System;

namespace IOBootstrap.NET.Common.Exceptions.Base
{
    [Serializable]
    public class IOServiceException : Exception
    {
        public int Code { get; }
        public String DetailedMessage { get; }

        public IOServiceException(int code, string message, string detailedMessage) : base(message)
        {
            this.Code = code;
            this.DetailedMessage = detailedMessage;
        }
    }
}
