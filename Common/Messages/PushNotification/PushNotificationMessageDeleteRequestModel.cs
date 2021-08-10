using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class PushNotificationMessageDeleteRequestModel: IORequestModel
    {

        [Required]
        public int ID { get; set; }

        public PushNotificationMessageDeleteRequestModel() : base()
        {
        }
    }
}
