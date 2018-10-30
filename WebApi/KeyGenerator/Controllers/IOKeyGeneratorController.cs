using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.WebApi.KeyGenerator.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;

namespace IOBootstrap.NET.WebApi.KeyGenerator.Controllers
{
    public class IOKeyGeneratorController<TLogger, TDBContext> : IOController<TLogger, IOViewModel<TDBContext>, TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOKeyGeneratorController(ILoggerFactory factory, 
                                        ILogger<TLogger> logger, 
                                        IConfiguration configuration, 
                                        TDBContext databaseContext,
                                        IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
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
