using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceDeleteRequestModel : IORequestModel
    {
        public int ID { get; set; }

        public IOResourceDeleteRequestModel() : base()
        {
        }
    }
}