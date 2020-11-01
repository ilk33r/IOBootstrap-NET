using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
	public class FirebaseResponseModel : IOModel
    {
        [JsonPropertyName("success")]
        public int Success { get; set; }

        [JsonPropertyName("failure")]
        public int Failure { get; set; }

        [JsonPropertyName("results")]
        public IList<FirebaseResponseResultModel> Results { get; set; }

		public FirebaseResponseModel() : base()
        {
        }
    }
}
