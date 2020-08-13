using System;
using IOBootstrap.NET.Common.Enumerations;

namespace IOBootstrap.NET.Common.Models.PushNotification
{
    public class PushNotificationModel
    {

		#region Properties
		
        public int ID { get; set; }
		public int AppBuildNumber { get; set; }
		public string AppBundleId { get; set; }
		public string AppVersion { get; set; }
		public int BadgeCount { get; set; }
		public string DeviceId { get; set; }
		public string DeviceName { get; set; }
		public string DeviceToken { get; set; }
		public DeviceTypes DeviceType { get; set; }
		public DateTimeOffset LastUpdateTime { get; set; }

        #endregion

    }
}
