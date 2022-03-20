using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Clients;

namespace IOBootstrap.NET.Common.Models.PushNotification
{
    public class PushNotificationDevicesModel : IOModel
    {

        public int ID { get; set; }
        public IOClientInfoModel Client { get; set; }
        public int BadgeCount { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public DeviceTypes DeviceType { get; set; }
        public IList<int> DeliveredMessages { get; set; }
    }
}
