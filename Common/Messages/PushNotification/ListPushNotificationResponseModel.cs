using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class ListPushNotificationResponseModel : IOResponseModel
    {        
        public IList<PushNotificationModel> Devices { get; set; }

        public ListPushNotificationResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        public ListPushNotificationResponseModel(int responseStatusMessage, IList<PushNotificationModel> devices) : base(responseStatusMessage)
        {
            this.Devices = devices;
        }
    }
}
