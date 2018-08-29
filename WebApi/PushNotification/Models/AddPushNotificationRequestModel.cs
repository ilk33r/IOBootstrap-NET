using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using System;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class AddPushNotificationRequestModel : IORequestModel
    {
		
		public int AppBuildNumber { get; set; }
		public String AppBundleId { get; set; }
		public String AppVersion { get; set; }
		public String DeviceId { get; set; }
		public String DeviceName { get; set; }
		public String DeviceToken { get; set; }
        public DeviceTypes DeviceType { get; set; }

    }
}
