using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.DataAccess.Entities;

public class PushNotificationMessageEntity
{
    #region Properties

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public int DeviceType { get; set; }

    [StringLength(64)]
    public string? NotificationCategory { get; set; }

    [StringLength(256)]
    public string? NotificationData { get; set; }

    [StringLength(256)]
    public string? NotificationMessage { get; set; }

    [StringLength(32)]
    public string? NotificationTitle { get; set; }

    [Required]
    [DefaultValue(false)]
    public bool IsCompleted { get; set; }

    [StringLength(255)]
    public string? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdateDate { get; set; }

    [ForeignKey("PushNotificationMessageID")]
    public ICollection<PushNotificationDeliveredMessagesEntity>? DeliveredMessages { get; set; }

    #endregion
}
