using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceAddRequestModel : IORequestModel
    {
        [Required]
        public string ResourceKey { get; set; }

        [Required]
        public string ResourceValue { get; set; }

        public IOResourceAddRequestModel() : base()
        {
        }
    }
}
