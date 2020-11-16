using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Handshake
{
    public class HandshakeResponseModel : IOResponseModel
    {

        public string KeyID { get; set; }
        public string PublicKeyExponent { get; set; }
        public string PublicKeyModulus { get; set; }

        public HandshakeResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        public HandshakeResponseModel(int responseStatusMessage, string publicKeyExponent, string publicKeyModulus, string keyID) : base(responseStatusMessage)
        {
            this.KeyID = keyID;
            this.PublicKeyExponent = publicKeyExponent;
            this.PublicKeyModulus = publicKeyModulus;
        }
    }
}
