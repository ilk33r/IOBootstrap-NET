using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.WebApi.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Linq;

namespace IOBootstrap.NET.WebApi.Authentication.Controllers
{
    public abstract class IOAuthenticationController<TLogger> : IOController<TLogger>
    {

        #region Controller Lifecycle

        public IOAuthenticationController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
            : base(factory, logger, configuration, database)
        {
        }

        #endregion

        #region Authentication Api

        [HttpPost]
        public IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || requestModel.ClientInfo == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), null, DateTime.Now);
            }

            // Check client 
            if (!this.CheckClient(requestModel.ClientInfo))
            {
                // Then return invalid clients
                this.Response.StatusCode = 400;
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), null, DateTime.Now);
            }

            // Obtain realm instance 
            Realm realm = _database.GetRealmForThread();

            // Obtain user entity
            var userEntities = realm.All<IOUserEntity>()
                                           .Where((arg1) => arg1.UserName == requestModel.UserName.ToLower());

            // Check user finded
            if (userEntities.Count() > 0)
            {
                // Obtain user entity
                IOUserEntity user = userEntities.First();

                // Check user password is wrong
                if (!IOCommonHelpers.VerifyPassword(requestModel.Password, user.Password))
                {
                    // Return response
                    this.Response.StatusCode = 400;
                    return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CREDIENTALS, "Invalid user."), null, DateTime.Now);
                }

                // Generate token for user
                string userTokenString = IOCommonHelpers.GenerateRandomAlphaNumericString(32);

                // Create decrypted user token string
                string decryptedUserToken = String.Format("{0}-{1}", user.ID, userTokenString);

                // Convert key and iv to byte array
                byte[] key = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionKey"));
                byte[] iv = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionIV"));

                // Encode user token
                byte[] userTokenData = IOCommonHelpers.EncryptStringToBytes(decryptedUserToken, key, iv);

                // Base 64 encode user token data
                string userToken = Convert.ToBase64String(userTokenData);

                // Create token date
                DateTime tokenDate = DateTime.UtcNow;

                // Obtain token life from configuration
                int tokenLife = _configuration.GetValue<int>("IOTokenLife");

                // Begin write transaction
                Transaction realmTransaction = realm.BeginWrite();

                // Update entity properties
                user.UserToken = userTokenString;
                user.TokenDate = tokenDate;

                // Delete all entity
                realm.Add(user, true);

                // Write transaction
                realmTransaction.Commit();

                // Dispose realm
                realm.Dispose();

                // Return response
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), userToken, tokenDate.Add(new TimeSpan(tokenLife)));
            }

            // Dispose realm
            realm.Dispose();

            // Return response
            this.Response.StatusCode = 400;
            return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CREDIENTALS, "Invalid user."), null, DateTime.Now);
        }

        #endregion

    }
}
