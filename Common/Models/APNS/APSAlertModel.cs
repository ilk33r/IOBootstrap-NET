using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APSAlertModel : IOModel
    {

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        public APSAlertModel(string title, string body)
        {
            this.Title = title;
            this.Body = body;
        }

    }
}
