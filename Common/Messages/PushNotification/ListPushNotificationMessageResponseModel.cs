using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;

namespace IOBootstrap.NET.Common.Messages.PushNotification;

public class ListPushNotificationMessageResponseModel : IOResponseModel
{

    public int Count { get; set; }
    public IList<PushNotificationMessageModel> Messages { get; set; }

    public ListPushNotificationMessageResponseModel(int count, IList<PushNotificationMessageModel> messages) : base()
    {
        this.Count = count;
        this.Messages = messages;
    }
}
