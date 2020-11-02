using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APNSJWTBodyModel : IOModel
    {

        [JsonPropertyName("iss")]
        public string TeamID { get; set; }

        [JsonPropertyName("iat")]
        public long Timestamp { get; set; }

        public APNSJWTBodyModel(string teamId, long timestamp)
        {
            this.TeamID = teamId;
            this.Timestamp = timestamp;
        }
    }
}
