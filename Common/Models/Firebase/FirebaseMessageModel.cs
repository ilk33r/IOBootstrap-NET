using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase;

public class FirebaseMessageModel : IOModel
{
    [JsonPropertyName("message")]
    public FirebaseModel? Message { get; set; }
}
