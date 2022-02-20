using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Clients
{
    public class IOClientInfoModel : IOModel
    {
        
		#region Properties

        public int Id { get; set; }
		public string ClientID { get; set; }
		public string ClientSecret { get; set; }
        public string ClientDescription { get; set; }
        public int IsEnabled { get; set; }
        public long RequestCount { get; set; }
        public long MaxRequestCount { get; set; }

		#endregion

		#region Initialization Methods

        public IOClientInfoModel() : base()
        {
        }

        public IOClientInfoModel(int id, string clientId, string clientSectet, string clientDescription, int isEnabled, long requestCount, long maxRequestCount) : base()
		{
            // Setup properties
            Id = id;
			ClientID = clientId;
			ClientSecret = clientSectet;
            ClientDescription = clientDescription;
            IsEnabled = isEnabled;
            RequestCount = requestCount;
            MaxRequestCount = maxRequestCount;
		}

		#endregion

	}
}
