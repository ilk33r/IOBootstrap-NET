using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.DataAccess.Entities;

public class PushNotificationDeliveredMessagesEntity
{

    #region Properties

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual IOPushNotificationDevicesEntity? Device { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual PushNotificationMessageEntity? PushNotificationMessage { get; set; }

    [DefaultValue(false)]
    public bool IsDelivered { get; set; }

    [Required]
    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? DeliverDate { get; set; }

    #endregion

}
