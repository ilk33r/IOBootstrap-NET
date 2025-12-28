using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase;

public class FirebaseResponseModel : IOModel
{
    public class DetailModel : IOModel
    {
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }
    }

    public class ErrorModel : IOModel
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("details")]
        public IList<DetailModel>? Details { get; set; }
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("error")]
    public ErrorModel? Error { get; set; }
}
