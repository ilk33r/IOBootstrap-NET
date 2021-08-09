using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageAddRequestModel : IORequestModel
    {

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTimeOffset MessageStartDate { get; set; }

        [Required]
        public DateTimeOffset MessageEndDate { get; set; }

        public IOMessageAddRequestModel() : base()
        {
        }
    }
}
