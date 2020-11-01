using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
    public class FirebaseResponseResultModel : IOModel
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("message_id")]
        public string MessageID { get; set; }
    }
}
