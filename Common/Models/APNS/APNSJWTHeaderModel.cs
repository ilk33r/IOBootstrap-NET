using System;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.APNS
{
    public class APNSJWTHeaderModel : IOModel
    {

        [JsonPropertyName("alg")]
        public string Alg { get; set; }

        [JsonPropertyName("kid")]
        public string KeyID { get; set; }

        public APNSJWTHeaderModel(string keyID)
        {
            this.Alg = "ES256";
            this.KeyID = keyID;
        }
    }
}
