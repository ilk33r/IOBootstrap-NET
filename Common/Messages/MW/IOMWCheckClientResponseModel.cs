using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWCheckClientResponseModel : IOResponseModel
    {
        public string ClientID { get; set; }
        
		public string ClientDescription { get; set; }

        public IOMWCheckClientResponseModel() : base()
        {
        }

        public IOMWCheckClientResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        public IOMWCheckClientResponseModel(string clientID, string clientDescription) : base()
        {
            ClientID = clientID;
            ClientDescription = clientDescription;
        }
    }
}
