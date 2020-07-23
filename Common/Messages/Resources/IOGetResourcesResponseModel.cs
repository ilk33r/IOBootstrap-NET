using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Resources;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOGetResourcesResponseModel : IOResponseModel
    {

        public IList<IOResourceModel> Resources { get; set; }

        public IOGetResourcesResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOGetResourcesResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        public IOGetResourcesResponseModel(int responseStatusMessage, IList<IOResourceModel> resources) : base(responseStatusMessage)
        {
            Resources = resources;
        }
    }
}
