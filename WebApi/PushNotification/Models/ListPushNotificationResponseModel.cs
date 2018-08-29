using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class ListPushNotificationResponseModel : IOResponseModel
    {
        
        public IList<PushNotificationModel> devices { get; set; }

        public ListPushNotificationResponseModel(IOResponseStatusModel status, IList<PushNotificationModel> devices) : base(status)
        {
            this.devices = devices;
        }
    }
}
