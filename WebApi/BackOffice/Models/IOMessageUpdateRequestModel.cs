using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMessageUpdateRequestModel : IOMessageAddRequestModel
    {

        public int MessageId { get; set; }

        public IOMessageUpdateRequestModel() : base()
        {
        }
    }
}
