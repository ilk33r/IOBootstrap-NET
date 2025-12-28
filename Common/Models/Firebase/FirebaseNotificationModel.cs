using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase;

public class FirebaseNotificationModel : IOModel
{

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    public FirebaseNotificationModel(string title, string body) : base()
    {
        this.Title = title;
        this.Body = body;
    }
}
