using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class ListPushNotificationRequestModel : IORequestModel
    {

        [Required]
        public int Start { get; set; }

        [Required]
        public int Limit { get; set; }

    }
}
