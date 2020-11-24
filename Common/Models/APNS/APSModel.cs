using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APSModel : IOModel
    {

        [JsonPropertyName("alert")]
        public APSAlertModel Alert { get; set; }

        [JsonPropertyName("sound")]
        public string Sound { get; set; }

        [JsonPropertyName("badge")]
        public int Badge { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        public APSModel(string title, string body, string sound, int badge, string category)
        {
            this.Alert = new APSAlertModel(title, body);
            this.Sound = sound;
            this.Badge = badge;
            this.Category = category;
        }
    }
}
