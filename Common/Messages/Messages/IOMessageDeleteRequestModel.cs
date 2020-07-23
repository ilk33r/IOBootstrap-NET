using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageDeleteRequestModel : IORequestModel
    {

        public int MessageId { get; set; }

        public IOMessageDeleteRequestModel() : base()
        {
        }
    }
}
