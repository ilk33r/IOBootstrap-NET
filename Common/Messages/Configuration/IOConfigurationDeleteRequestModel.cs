using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationDeleteRequestModel : IORequestModel
    {

        [Required]
        public int ConfigId { get; set; }

        public IOConfigurationDeleteRequestModel() : base()
        {
        }
    }
}
