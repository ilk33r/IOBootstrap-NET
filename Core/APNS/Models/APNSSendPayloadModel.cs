using System;

namespace IOBootstrap.NET.Core.APNS.Utils.Models
{
    public class APNSSendPayloadModel
    {

        public APNSPayloadModel payload { get; set; }
		public string token { get; set; }

		public APNSSendPayloadModel(int badge, string body, string title, string token)
		{
            this.payload = new APNSPayloadModel(badge, body, title);
			this.token = token;
		}
    }
}
