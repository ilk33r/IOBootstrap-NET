using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientUpdateRequestModel: IORequestModel
    {

        #region properties

        public int ClientId { get; set; }
        public string ClientDescription { get; set; }
        public int IsEnabled { get; set; }
        public long RequestCount { get; set; }
        public long MaxRequestCount { get; set; }

        #endregion
    }
}
