using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWPushNotificationDevicesRequestModel : IORequestModel
    {
        public DeviceTypes DeviceType { get; set; }
        public int MessageId { get; set; }
        public int? ClientId { get; set; }
    }
}
