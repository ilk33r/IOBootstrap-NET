using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class SendPushNotificationRequestModel : IORequestModel
    {

        public int? ClientId { get; set; }
        public DeviceTypes DeviceType { get; set; }
        public string NotificationCategory { get; set; }
        public string NotificationData { get; set; }

        [Required]
        public string NotificationMessage { get; set; }
        public string NotificationTitle { get; set; }
    }
}
