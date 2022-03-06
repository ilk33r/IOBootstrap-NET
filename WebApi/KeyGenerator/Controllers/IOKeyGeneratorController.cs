using System;
using System.Security.Cryptography;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Common.Messages.KeyGenerator;
using IOBootstrap.NET.WebApi.KeyGenerator.ViewModels;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Common.Logger;
using Microsoft.AspNetCore.Mvc;
using IOBootstrap.Net.Common.Messages.KeyGenerator;
using IOBootstrap.NET.Common.Attributes;

namespace IOBootstrap.NET.WebApi.KeyGenerator.Controllers
{
    public class IOKeyGeneratorController : IOController<IOKeyGeneratorViewModel>
    {

        #region Controller Lifecycle

        public IOKeyGeneratorController(IConfiguration configuration, 
                                        IWebHostEnvironment environment,
                                        ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        public IOKeyGeneratorResponseModel GenerateKeys() 
        {
            // Generate keys
            string authorizationKey = IORandomUtilities.GenerateRandomAlphaNumericString(32);

            // Create encryptor
            Aes encryptor = Aes.Create();

			// Generate key and iv
            encryptor.GenerateKey();
			encryptor.GenerateIV();

            // Convert key and iv to string
            string encryptionKey = Convert.ToBase64String(encryptor.Key);
            string encryptionIV = Convert.ToBase64String(encryptor.IV);

            // Create and return response
            return new IOKeyGeneratorResponseModel(authorizationKey, encryptionKey, encryptionIV);
        }

        [IOValidateRequestModel]
        [HttpPost]
        public IOEncryptResponseModel Encrypt([FromBody] IOEncryptRequestModel requestModel)
        {
            return ViewModel.Encrypt(requestModel);
        }

    }
}
