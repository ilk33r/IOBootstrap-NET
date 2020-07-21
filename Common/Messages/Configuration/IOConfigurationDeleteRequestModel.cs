using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationDeleteRequestModel : IORequestModel
    {

        public int ConfigId { get; set; }

        public IOConfigurationDeleteRequestModel() : base()
        {
        }
    }
}
