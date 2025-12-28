using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase;

public class FirebaseDataModel : IOModel
{
    [JsonPropertyName("notificationType")]
    public string? NotificationType { get; set; }

    [JsonPropertyName("notificationId")]
    public string? NotificationId { get; set; }

    [JsonPropertyName("customData")]
    public string? CustomData { get; set; }

    [JsonPropertyName("badgeCount")]
    public string? BadgeCount { get; set; }

    public FirebaseDataModel(string notificationType, int notificationId, string customData, int badgeCount) : base()
    {
        this.NotificationType = notificationType;
        this.NotificationId = notificationId.ToString();
        this.CustomData = customData;
        this.BadgeCount = badgeCount.ToString();
    }
}
