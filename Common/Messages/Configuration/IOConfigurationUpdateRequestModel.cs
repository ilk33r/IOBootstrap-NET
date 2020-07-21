using System;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationUpdateRequestModel : IOConfigurationAddRequestModel
    {

        public int ConfigId { get; set; }

		public IOConfigurationUpdateRequestModel() : base()
        {
        }
    }
}
