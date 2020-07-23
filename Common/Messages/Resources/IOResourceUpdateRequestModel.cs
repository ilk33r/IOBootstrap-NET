using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceUpdateRequestModel : IORequestModel
    {
        public int ResourceID { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceUpdateRequestModel() : base()
        {
        }
    }
}