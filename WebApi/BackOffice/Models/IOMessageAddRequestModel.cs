using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
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
