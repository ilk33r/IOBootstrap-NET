using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    
    public class AddPushNotificationResponseModel : IOResponseModel
    {
        public AddPushNotificationResponseModel(IOResponseStatusModel status) : base(status)
        {
		}
    }
}
