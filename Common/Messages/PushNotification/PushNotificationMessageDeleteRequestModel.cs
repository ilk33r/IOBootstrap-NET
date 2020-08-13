using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class PushNotificationMessageDeleteRequestModel: IORequestModel
    {

        public int ID { get; set; }

        public PushNotificationMessageDeleteRequestModel() : base()
        {
        }
    }
}
