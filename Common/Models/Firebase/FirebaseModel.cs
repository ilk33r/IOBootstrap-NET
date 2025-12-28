using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase;

public class FirebaseModel : IOModel
{

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("notification")]
    public FirebaseNotificationModel Notification { get; set; }

    [JsonPropertyName("data")]
    public FirebaseDataModel Data { get; set; }

    public FirebaseModel(string token, string title, string body, string notificationType, int notificationId, string customData, int badgeCount) : base()
    {
        this.Token = token;
        this.Notification = new FirebaseNotificationModel(title, body);
        this.Data = new FirebaseDataModel(notificationType, notificationId, customData, badgeCount);
    }
}
