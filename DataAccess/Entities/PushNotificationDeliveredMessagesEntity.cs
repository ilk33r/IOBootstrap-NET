using System;
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

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual PushNotificationEntity? PushNotification { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual PushNotificationMessageEntity? PushNotificationMessage { get; set; }

    #endregion

}
