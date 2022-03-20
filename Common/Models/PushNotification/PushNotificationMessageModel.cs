using System;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Clients;

namespace IOBootstrap.NET.Common.Models.PushNotification
{
    public class PushNotificationMessageModel : IOModel
    {
    
        public int ID { get; set; }
        public IOClientInfoModel Client { get; set; }
        public int DeviceType { get; set; }
        public string NotificationCategory { get; set; }
        public string NotificationData { get; set; }
        public DateTimeOffset NotificationDate { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationTitle { get; set; }
        public int IsCompleted { get; set; }
        public PushNotificationModel PushNotificationDeviceID { get; set; }

        public PushNotificationMessageModel() : base()
        {
        }
    }
}
