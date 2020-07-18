using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

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

        public IOKeyGeneratorResponseModel(IOResponseStatusModel status, string authorizationKey, string encryptionKey, string encryptionIV) : base(status)
		{
            // Setup properties
            this.AuthorizationKey = authorizationKey;
            this.EncryptionKey = encryptionKey;
            this.EncryptionIV = encryptionIV;
		}

		#endregion
	}
}
