using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APNSPayloadModel : IOModel
    {

        [JsonPropertyName("aps")]
        public APSModel APS { get; set; }

        [JsonPropertyName("customData")]
        public string CustomData { get; set; }

        public APNSPayloadModel(string title, string body, int badge, string customData, string category)
        {
            this.APS = new APSModel(title, body, "default", badge, category);
            this.CustomData = customData;
        }
    }
}
