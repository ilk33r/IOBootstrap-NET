using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOGetResourcesRequestModel : IORequestModel
    {

        public IList<string> ResourceKeys { get; set; }

        public IOGetResourcesRequestModel() : base()
        {
        }
    }
}
