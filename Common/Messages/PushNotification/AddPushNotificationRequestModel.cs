using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class AddPushNotificationRequestModel : IORequestModel
    {
		[Required]
		public int AppBuildNumber { get; set; }

		[Required]
		[StringLength(64)]
		public String AppBundleId { get; set; }

		[Required]
		[StringLength(10)]
		public String AppVersion { get; set; }

		[Required]
		[StringLength(128)]
		public String DeviceId { get; set; }

		[Required]
		[StringLength(128)]
		public String DeviceName { get; set; }

		[Required]
		[StringLength(512)]
		public String DeviceToken { get; set; }

		[Required]
        public DeviceTypes DeviceType { get; set; }

    }
}
