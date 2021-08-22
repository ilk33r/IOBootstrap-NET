using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.KeyGenerator
{
    public class IOKeyGeneratorResponseModel : IOResponseModel
    {

        #region Properties

        public string AuthorizationKey { get; set; }
        public string EncryptionKey { get; set; }
        public string EncryptionIV { get; set; }

		#endregion

		#region Initialization Methods

        public IOKeyGeneratorResponseModel(string authorizationKey, string encryptionKey, string encryptionIV) : base()
		{
            // Setup properties
            this.AuthorizationKey = authorizationKey;
            this.EncryptionKey = encryptionKey;
            this.EncryptionIV = encryptionIV;
		}

		#endregion
	}
}
