using System;
using System.Security.Cryptography;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Messages.KeyGenerator;
using IOBootstrap.NET.WebApi.KeyGenerator.ViewModels;
using IOBootstrap.NET.Core.Logger;

namespace IOBootstrap.NET.WebApi.KeyGenerator.Controllers
{
    public class IOKeyGeneratorController : IOController<IOKeyGeneratorViewModel, IODatabaseContextDefaultImpl>
    {

        #region Controller Lifecycle

        public IOKeyGeneratorController(IConfiguration configuration, 
                                        IODatabaseContextDefaultImpl databaseContext,
                                        IWebHostEnvironment environment,
                                        ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        public IOKeyGeneratorResponseModel GenerateKeys() 
        {
            // Generate keys
            string authorizationKey = IORandomUtilities.GenerateRandomAlphaNumericString(32);

            // Create encryptor
            RijndaelManaged encryptor = new RijndaelManaged();

			// Generate key and iv
            encryptor.GenerateKey();
			encryptor.GenerateIV();

            // Convert key and iv to string
            string encryptionKey = Convert.ToBase64String(encryptor.Key);
            string encryptionIV = Convert.ToBase64String(encryptor.IV);

            // Create and return response
            return new IOKeyGeneratorResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), authorizationKey, encryptionKey, encryptionIV);
        }

    }
}
