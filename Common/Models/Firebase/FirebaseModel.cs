using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
	public class FirebaseModel : IOModel
    {

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("data")]
        public FirebaseDataModel Data { get; set; }

        public FirebaseModel(string token, string title, string message, string notificationType, int notificationId, string customData) : base()
        {
            this.To = token;
            this.Data = new FirebaseDataModel(title, message, notificationType, notificationId, customData);
        }
    }
}
