using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.WebApi.PushNotification.Entities
{
    
    public class PushNotificationEntity
    {

		#region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        public int AppBuildNumber { get; set; }
        public string AppBundleId { get; set; }
        public string AppVersion { get; set; }
        public int BadgeCount { get; set; }

		[StringLength(128)]
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }
        public string DeviceToken { get; set; }
        public int DeviceType { get; set; }
        public DateTimeOffset LastUpdateTime { get; set; }

		#endregion

	}
}
