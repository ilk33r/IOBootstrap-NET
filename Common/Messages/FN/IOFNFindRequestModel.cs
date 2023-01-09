using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.FN
{
    public class IOFNFindRequestModel : IORequestModel
    {
        [Required]
        public int ID { get; set; }
    }
}
