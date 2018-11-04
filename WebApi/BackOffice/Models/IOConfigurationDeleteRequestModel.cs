using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
	public class IOConfigurationDeleteRequestModel : IORequestModel
    {

        public int ConfigId { get; set; }

        public IOConfigurationDeleteRequestModel() : base()
        {
        }
    }
}
