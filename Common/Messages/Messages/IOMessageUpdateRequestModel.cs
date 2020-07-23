using System;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageUpdateRequestModel : IOMessageAddRequestModel
    {

        public int MessageId { get; set; }

        public IOMessageUpdateRequestModel() : base()
        {
        }
    }
}
