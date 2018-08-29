using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Enumerations;
using System;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class SendPushNotificationRequestModel : IORequestModel
    {

        public DeviceTypes DeviceType { get; set; }
        public string NotificationCategory { get; set; }
        public string NotificationData { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationTitle { get; set; }
    }
}
