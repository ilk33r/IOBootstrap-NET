using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationListResponseModel : IOResponseModel
    {

        public IList<IOConfigurationModel> Configurations { get; set; }

        public IOConfigurationListResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOConfigurationListResponseModel(int responseStatusMessage, IList<IOConfigurationModel> configurations) : base(responseStatusMessage)
        {
            Configurations = configurations;
        }
    }
}
