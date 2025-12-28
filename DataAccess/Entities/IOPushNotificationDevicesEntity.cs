using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IOBootstrap.NET.Common.Enumerations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.DataAccess.Entities;

public abstract class IOPushNotificationDevicesEntity
{

    #region Properties

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public int AppBuildNumber { get; set; }

    [StringLength(64)]
    public string? AppBundleId { get; set; }

    [StringLength(10)]
    public string? AppVersion { get; set; }

    public int BadgeCount { get; set; }

    [DefaultValue(0)]
    public int WrongAttemptCount { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    [StringLength(128)]
    public string? DeviceId { get; set; }

    [StringLength(128)]
    public string? DeviceName { get; set; }

    [StringLength(512)]
    public string? DeviceToken { get; set; }

    public DeviceTypes DeviceType { get; set; }

    public DateTimeOffset LastUpdateTime { get; set; }

    [ForeignKey("PushNotificationDeviceID")]
    public ICollection<PushNotificationDeliveredMessagesEntity>? DeliveredMessages { get; set; }

    #endregion

}
