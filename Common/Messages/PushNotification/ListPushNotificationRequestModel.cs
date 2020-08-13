using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class ListPushNotificationRequestModel : IORequestModel
    {

        public int Start { get; set; }
        public int Limit { get; set; }

    }
}
