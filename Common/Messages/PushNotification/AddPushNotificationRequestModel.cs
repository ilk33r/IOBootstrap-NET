using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification;

public class AddPushNotificationRequestModel : IORequestModel
{
	[Required]
	public int? AppBuildNumber { get; set; }

	[Required]
	[StringLength(256)]
	public String? EncryptedAppBundleId { get; set; }

	[Required]
	[StringLength(10)]
	public String? AppVersion { get; set; }

	[Required]
	[StringLength(512)]
	public String? EncryptedDeviceId { get; set; }

	[Required]
	[StringLength(512)]
	public String? EncryptedDeviceName { get; set; }

	[Required]
	[StringLength(1024)]
	public String? EncryptedDeviceToken { get; set; }

	[Required]
	public DeviceTypes? DeviceType { get; set; }

}
