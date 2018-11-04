using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class PushNotificationMessageDeleteRequestModel: IORequestModel
    {

        public int ID { get; set; }

        public PushNotificationMessageDeleteRequestModel() : base()
        {
        }
    }
}
