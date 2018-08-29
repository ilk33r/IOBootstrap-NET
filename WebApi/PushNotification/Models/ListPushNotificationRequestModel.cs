using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.PushNotification.Models
{
    public class ListPushNotificationRequestModel : IORequestModel
    {

        public int Start { get; set; }
        public int Limit { get; set; }

    }
}
