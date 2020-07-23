using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientAddRequestModel : IORequestModel
    {

        public string ClientDescription { get; set; }
        public long RequestCount { get; set; }

    }
}
