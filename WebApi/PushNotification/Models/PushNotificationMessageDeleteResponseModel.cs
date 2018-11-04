using System;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class PushNotificationMessageDeleteResponseModel : IOResponseModel
    {
        public PushNotificationMessageDeleteResponseModel(IOResponseStatusModel status) : base(status)
        {
        }
    }
}
