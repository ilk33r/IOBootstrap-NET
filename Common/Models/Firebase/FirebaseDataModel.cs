using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
    public class FirebaseDataModel: IOModel
    {

        public string title { get; set; }
        public string message { get; set; }
        public string notificationType { get; set; }
        public int notificationId { get; set; }

        public FirebaseDataModel(string title, string message, string notificationType, int notificationId) : base()
        {
            this.title = title;
            this.message = message;
            this.notificationType = notificationType;
            this.notificationId = notificationId;
        }
    }
}
