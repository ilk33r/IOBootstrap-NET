using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOGetResourcesRequestModel : IORequestModel
    {

        [Required]
        [MinLength(1)]
        public IList<string> ResourceKeys { get; set; }

        public IOGetResourcesRequestModel() : base()
        {
        }
    }
}
