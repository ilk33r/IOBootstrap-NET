using System;

namespace IOBootstrap.NET.Common.Models.Clients
{
    public class IOClientInfoModel
    {
        
		#region Properties

        public int Id { get; }
		public string ClientID { get; }
		public string ClientSecret { get; }
        public string ClientDescription { get; }
        public int IsEnabled { get; }
        public long RequestCount { get; }
        public long MaxRequestCount { get; }

		#endregion

		#region Initialization Methods

        public IOClientInfoModel(int id, string clientId, string clientSectet, string clientDescription, int isEnabled, long requestCount, long maxRequestCount)
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
