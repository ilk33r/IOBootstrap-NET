using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Resources;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOGetResourcesResponseModel : IOResponseModel
    {

        public IList<IOResourceModel> Resources { get; set; }

        public IOGetResourcesResponseModel(IList<IOResourceModel> resources) : base()
        {
            Resources = resources;
        }
    }
}
