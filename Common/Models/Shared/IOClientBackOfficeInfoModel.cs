using System;

namespace IOBootstrap.NET.Common.Models.Shared
{
    public class IOClientBackOfficeInfoModel
    {
        
		#region Properties

        public int Id { get; }
		public string ClientID { get; }
		public string ClientSecret { get; }

		#endregion

		#region Initialization Methods

		public IOClientBackOfficeInfoModel(int id, string clientId, string clientSectet)
		{
            // Setup properties
            this.Id = id;
			this.ClientID = clientId;
			this.ClientSecret = clientSectet;
		}

		#endregion

	}
}
