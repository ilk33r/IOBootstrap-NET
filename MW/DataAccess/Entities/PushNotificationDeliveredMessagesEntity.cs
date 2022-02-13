using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.MW.DataAccess.Entities
{
    public class PushNotificationDeliveredMessagesEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual PushNotificationEntity PushNotification { get; set; }
        
        public virtual PushNotificationMessageEntity PushNotificationMessage { get; set; }

        #endregion

    }
}
