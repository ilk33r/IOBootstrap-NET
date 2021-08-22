using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class ListPushNotificationResponseModel : IOResponseModel
    {        
        public IList<PushNotificationModel> Devices { get; set; }

        public ListPushNotificationResponseModel(IList<PushNotificationModel> devices) : base()
        {
            this.Devices = devices;
        }
    }
}
