using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APNSResponseModel : IOModel
    {
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
