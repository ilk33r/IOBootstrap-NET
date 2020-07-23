using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Messages
{
    public class IOMessageModel : IOModel
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTimeOffset MessageCreateDate { get; set; }
        public DateTimeOffset MessageStartDate { get; set; }
        public DateTimeOffset MessageEndDate { get; set; }
    }
}
