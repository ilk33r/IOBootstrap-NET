using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.PushNotification
{
    public class PushNotificationDeliveredMessageModel : IOModel
    {
        public int PushNotificationID { get; set; }
        public int PushNotificationMessageID { get; set; }
    }
}
