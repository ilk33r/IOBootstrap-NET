using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Menu
{
    public class IOMenuDeleteRequestModel : IORequestModel
    {
        [Required]
        public int ID { get; set; }

        public IOMenuDeleteRequestModel() : base()
        {
        }
    }
}
