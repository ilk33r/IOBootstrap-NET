using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientAddRequestModel : IORequestModel
    {

        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string ClientDescription { get; set; }

        [Required]
        public long RequestCount { get; set; }

    }
}
