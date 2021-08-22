using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class ListPushNotificationMessageResponseModel : IOResponseModel
    {

        public IList<PushNotificationMessageModel> Messages { get; set; }

        public ListPushNotificationMessageResponseModel(IList<PushNotificationMessageModel> messages) : base()
        {
            this.Messages = messages;
        }
    }
}
