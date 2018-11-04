using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class ListPushNotificationMessageResponseModel : IOResponseModel
    {

        public IList<PushNotificationMessageModel> Messages { get; set; }

        public ListPushNotificationMessageResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public ListPushNotificationMessageResponseModel(IOResponseStatusModel status, IList<PushNotificationMessageModel> messages) : base(status)
        {
            this.Messages = messages;
        }
    }
}
