using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Entities.Configuration;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
	public class IOConfigurationListResponseModel : IOResponseModel
    {

        public IList<IOConfigurationEntity> Configurations { get; set; }

        public IOConfigurationListResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOConfigurationListResponseModel(IOResponseStatusModel status, IList<IOConfigurationEntity> configurations) : base(status)
        {
            this.Configurations = configurations;
        }
    }
}
