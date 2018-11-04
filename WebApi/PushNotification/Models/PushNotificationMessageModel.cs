using System;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class PushNotificationMessageModel : IOModel
    {
    
        public int ID { get; set; }
        public IOClientsEntity Client { get; set; }
        public int DeviceType { get; set; }
        public string NotificationCategory { get; set; }
        public string NotificationData { get; set; }
        public DateTimeOffset NotificationDate { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationTitle { get; set; }
        public int IsCompleted { get; set; }
        public int SendedDevices { get; set; }

        public PushNotificationMessageModel() : base()
        {
        }
    }
}
