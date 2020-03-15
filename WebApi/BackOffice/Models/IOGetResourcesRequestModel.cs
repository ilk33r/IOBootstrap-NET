using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOGetResourcesRequestModel : IORequestModel
    {

        public IList<string> ResourceKeys { get; set; }

        public IOGetResourcesRequestModel() : base()
        {
        }
    }
}
