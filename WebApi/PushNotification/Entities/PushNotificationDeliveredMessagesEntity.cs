using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.WebApi.PushNotification.Entities
{
    public class PushNotificationDeliveredMessagesEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual PushNotificationEntity pushNotificationEntity { get; set; }
        public virtual PushNotificationMessageEntity pushNotificationMessageEntity { get; set; }

        #endregion

    }
}
