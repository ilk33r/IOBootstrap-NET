using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Configuration;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
	public class IOConfigurationListResponseModel : IOResponseModel
    {

        public IList<IOConfigurationModel> Configurations { get; set; }

        public IOConfigurationListResponseModel(IList<IOConfigurationModel> configurations) : base()
        {
            Configurations = configurations;
        }
    }
}
