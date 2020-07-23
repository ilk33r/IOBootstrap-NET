using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceAddRequestModel : IORequestModel
    {
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceAddRequestModel() : base()
        {
        }
    }
}
