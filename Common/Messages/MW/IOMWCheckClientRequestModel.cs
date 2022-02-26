using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.Net.Common.Messages.MW
{
    public class IOMWCheckClientRequestModel : IORequestModel
    {
        public string ClientID { get; set; }
        
		public string ClientSecret { get; set; }
    }
}
