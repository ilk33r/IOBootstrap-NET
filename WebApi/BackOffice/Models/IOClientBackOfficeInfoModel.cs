﻿using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOClientBackOfficeInfoModel
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

        public IOClientBackOfficeInfoModel(int id, string clientId, string clientSectet, string clientDescription, int isEnabled, long requestCount, long maxRequestCount)
		{
            // Setup properties
            this.Id = id;
			this.ClientID = clientId;
			this.ClientSecret = clientSectet;
            this.ClientDescription = clientDescription;
            this.IsEnabled = isEnabled;
            this.RequestCount = requestCount;
            this.MaxRequestCount = maxRequestCount;
		}

		#endregion

	}
}
