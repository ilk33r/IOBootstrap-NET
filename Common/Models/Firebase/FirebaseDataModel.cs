using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
    public class FirebaseDataModel: IOModel
    {

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("notificationType")]
        public string NotificationType { get; set; }

        [JsonPropertyName("notificationId")]
        public int NotificationId { get; set; }

        public FirebaseDataModel(string title, string message, string notificationType, int notificationId) : base()
        {
            this.Title = title;
            this.Message = message;
            this.NotificationType = notificationType;
            this.NotificationId = notificationId;
        }
    }
}
