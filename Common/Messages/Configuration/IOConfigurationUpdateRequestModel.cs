using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationUpdateRequestModel : IOConfigurationAddRequestModel
    {

        [Required]
        public int ConfigId { get; set; }

		public IOConfigurationUpdateRequestModel() : base()
        {
        }
    }
}
