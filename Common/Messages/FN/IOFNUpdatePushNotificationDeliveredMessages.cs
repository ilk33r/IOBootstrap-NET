using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.FN
{
    public class IOFNUpdatePushNotificationDeliveredMessages : IORequestModel
    {
        [Required]
        public IList<PushNotificationDevicesModel> InvalidDevices { get; set; }

        [Required]
        public IList<PushNotificationDeliveredMessageModel> DeliveredMessages { get; set; }
    }
}
