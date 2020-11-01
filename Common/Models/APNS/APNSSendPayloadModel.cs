using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APNSSendPayloadModel : IOModel
    {
        public APNSPayloadModel Payload { get; set; }

		public string DeviceToken { get; set; }

		public APNSSendPayloadModel(string title, string body, int badge, string deviceToken)
		{
            this.Payload = new APNSPayloadModel(title, body, badge);
			this.DeviceToken = deviceToken;
		}
    }
}
