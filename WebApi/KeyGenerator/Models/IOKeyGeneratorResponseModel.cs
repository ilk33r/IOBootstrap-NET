using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.KeyGenerator.Models
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
