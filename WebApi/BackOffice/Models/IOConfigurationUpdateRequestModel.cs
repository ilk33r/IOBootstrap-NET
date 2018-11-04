using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
	public class IOConfigurationUpdateRequestModel : IOConfigurationAddRequestModel
    {

        public int ConfigId { get; set; }

		public IOConfigurationUpdateRequestModel() : base()
        {
        }
    }
}
