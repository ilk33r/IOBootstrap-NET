using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.PushNotification
{
    
    public class AddPushNotificationResponseModel : IOResponseModel
    {
        public AddPushNotificationResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
