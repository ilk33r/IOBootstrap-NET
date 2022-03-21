using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWCheckClientRequestModel : IORequestModel
    {
        public string ClientID { get; set; }
        
		public string ClientSecret { get; set; }
    }
}
