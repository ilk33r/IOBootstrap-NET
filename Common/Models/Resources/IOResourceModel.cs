using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Resources
{
    public class IOResourceModel : IOModel
    {

        public int ResourceID { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceModel() : base()
        {
        }
    }
}
