using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IOBootstrap.NET.Common.Enumerations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.MW.DataAccess.Entities
{
    
    public class PushNotificationEntity
    {

		#region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        public IOClientsEntity Client { get; set; }

        public int AppBuildNumber { get; set; }

        [StringLength(64)]
        public string AppBundleId { get; set; }

        [StringLength(10)]
        public string AppVersion { get; set; }

        public int BadgeCount { get; set; }

		[StringLength(128)]
        public string DeviceId { get; set; }

        [StringLength(128)]
        public string DeviceName { get; set; }

        [StringLength(512)]
        public string DeviceToken { get; set; }

        public DeviceTypes DeviceType { get; set; }

        public DateTimeOffset LastUpdateTime { get; set; }

        [ForeignKey("PushNotificationID")]
        public ICollection<PushNotificationDeliveredMessagesEntity> DeliveredMessages { get; set; }

		#endregion

	}
}
