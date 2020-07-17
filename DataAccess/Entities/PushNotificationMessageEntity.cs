using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.DataAccess.Entities
{
    public class PushNotificationMessageEntity
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public IOClientsEntity Client { get; set; }

        public int DeviceType { get; set; }

        [StringLength(64)]
        public string NotificationCategory { get; set; }

        [StringLength(256)]
        public string NotificationData { get; set; }

        public DateTimeOffset NotificationDate { get; set; }

        [StringLength(256)]
        public string NotificationMessage { get; set; }

        [StringLength(32)]
        public string NotificationTitle { get; set; }

        public int IsCompleted { get; set; }

        #endregion
    }
}
