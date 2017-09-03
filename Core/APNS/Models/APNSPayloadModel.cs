using System;

namespace IOBootstrap.NET.Core.APNS.Utils.Models
{
    public class APNSPayloadModel
    {

        public APSModel aps { get; set; }

        public APNSPayloadModel(int badge, string body, string title)
        {
            this.aps = new APSModel();
            this.aps.alert = new APSAlertModel();
            this.aps.alert.body = body;
            this.aps.alert.title = title;
			this.aps.badge = badge;
            this.aps.sound = "default";
        }
    }
}
