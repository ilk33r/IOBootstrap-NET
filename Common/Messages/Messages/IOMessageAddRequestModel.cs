using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageAddRequestModel : IORequestModel
    {

        public string Message { get; set; }
        public DateTimeOffset MessageStartDate { get; set; }
        public DateTimeOffset MessageEndDate { get; set; }

        public IOMessageAddRequestModel() : base()
        {
        }
    }
}
