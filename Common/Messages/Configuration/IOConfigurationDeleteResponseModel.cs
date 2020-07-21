using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationDeleteResponseModel : IOResponseModel
    {
        public IOConfigurationDeleteResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOConfigurationDeleteResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
