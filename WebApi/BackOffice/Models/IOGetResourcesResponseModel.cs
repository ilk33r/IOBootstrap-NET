using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOGetResourcesResponseModel : IOResponseModel
    {

        public IList<IOResourceModel> Resources { get; set; }

        public IOGetResourcesResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOGetResourcesResponseModel(IOResponseStatusModel status, IList<IOResourceModel> resources) : base(status)
        {
            Resources = resources;
        }
    }
}
