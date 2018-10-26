using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMessageModel : IOModel
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTimeOffset MessageCreateDate { get; set; }
    }
}
