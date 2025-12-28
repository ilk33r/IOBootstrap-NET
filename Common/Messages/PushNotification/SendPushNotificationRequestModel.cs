using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification;

public class SendPushNotificationRequestModel : IORequestModel
{
    public DeviceTypes DeviceType { get; set; }
    
    [IOBackofficeRequest]
    public string? NotificationCategory { get; set; }
    
    [IOBackofficeRequest]
    public string? NotificationData { get; set; }

    [Required]
    [MaxLength(512)]
    public string? NotificationMessage { get; set; }

    [MaxLength(128)]
    public string? NotificationTitle { get; set; }
}
