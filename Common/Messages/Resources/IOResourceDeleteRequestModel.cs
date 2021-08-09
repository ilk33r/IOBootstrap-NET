using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceDeleteRequestModel : IORequestModel
    {
        [Required]
        public int ID { get; set; }

        public IOResourceDeleteRequestModel() : base()
        {
        }
    }
}