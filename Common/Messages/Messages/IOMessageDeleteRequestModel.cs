using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageDeleteRequestModel : IORequestModel
    {

        [Required]
        public int MessageId { get; set; }

        public IOMessageDeleteRequestModel() : base()
        {
        }
    }
}
