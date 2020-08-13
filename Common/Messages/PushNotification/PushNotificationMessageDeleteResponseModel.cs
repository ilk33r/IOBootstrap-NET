using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    public class PushNotificationMessageDeleteResponseModel : IOResponseModel
    {
        public PushNotificationMessageDeleteResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public PushNotificationMessageDeleteResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
