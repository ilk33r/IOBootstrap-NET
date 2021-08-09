using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageUpdateRequestModel : IOMessageAddRequestModel
    {

        [Required]
        public int MessageId { get; set; }

        public IOMessageUpdateRequestModel() : base()
        {
        }
    }
}
