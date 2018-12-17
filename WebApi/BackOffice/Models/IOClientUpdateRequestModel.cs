using IOBootstrap.NET.Common.Models.BaseModels;
using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
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
