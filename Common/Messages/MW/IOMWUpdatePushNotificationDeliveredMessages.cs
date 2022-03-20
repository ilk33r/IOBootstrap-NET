using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWUpdatePushNotificationDeliveredMessages : IORequestModel
    {
        public IList<PushNotificationDevicesModel> InvalidDevices { get; set; }
        public IList<PushNotificationDeliveredMessageModel> DeliveredMessages { get; set; }
    }
}
