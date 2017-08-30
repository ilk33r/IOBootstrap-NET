using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeController<TLogger>: IOController<TLogger>
    {

        #region Controller Lifecycle

        public IOBackOfficeController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
            : base(factory, logger, configuration, database)
        {
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Check is not back office
            if (!this.IsBackOffice())
            {
                // Obtain response model
                IOResponseModel responseModel = this.Error400("Restricted page.");

                // Override response
                context.Result = new JsonResult(responseModel);

                // Do nothing
                return;
            }

            base.OnActionExecuting(context);
        }

        #endregion

        #region Client Methods

        [HttpPost]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Create a client entity
            IOClientsEntity clientEntity = IOClientsEntity.CreateClient(_database, requestModel.ClientDescription);

            // Write client to database
            _database.InsertEntity(clientEntity)
                     .Subscribe();

            // Create client info
            var clientInfos = new IOClientBackOfficeInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret);

            // Create and return response
            return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
		}

        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel) 
        {
            // Obtain realm instance 
            Realm realm = _database.GetRealmForMainThread();

            // Obtain client entity
            IOClientsEntity clientEntity = realm.Find<IOClientsEntity>(requestModel.ClientId);

            // Check client entity is not null
            if (clientEntity != null) {
				// Begin write transaction
				Transaction realmTransaction = realm.BeginWrite();

				// Delete all entity
				realm.Remove(clientEntity);

				// Write transaction
				realmTransaction.Commit();

                // Then return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Dispose realm
            realm.Dispose();

            // Return bad request
            this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            var clientInfos = IOClientsEntity.GetClients(_database);

            // Create and return response
            return new IOClientListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
        }

        #endregion

        #region Helper Methods

        public bool IsBackOffice()
        {
            // Check back office is open and token exists
            if (_configuration.GetValue<bool>("IOBackOfficeIsOpen"))
            {
                // Check authorization token exists
                if (this.Request.Headers.ContainsKey("X-IO-AUTHORIZATION-TOKEN")) 
                {
					// Obtain request authorization value
					string requestAuthorization = this.Request.Headers["X-IO-AUTHORIZATION-TOKEN"];

					// Convert key and iv to byte array
					byte[] key = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionKey"));
					byte[] iv = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionIV"));

                    // Obtain decrypted token value
                    string decryptedToken = IOCommonHelpers.DecryptStringFromBytes(Convert.FromBase64String(requestAuthorization), key, iv);

                    // Split user id and token value
                    string[] tokenData = decryptedToken.Split('-');

                    // Obtain user id from token data
                    int userId = int.Parse(tokenData[0]);

                    // Check token data is correct
                    if (tokenData.Count() > 1) {
						// Obtain realm instance
						Realm realm = _database.GetRealmForThread();

						// Obtain user entity from database
						IOUserEntity userEntity = realm.Find<IOUserEntity>(userId);

						// Check user entity is not null
						if (userEntity != null)
						{
                            // Obtain token life from configuration
                            int tokenLife = _configuration.GetValue<int>("IOTokenLife");

                            // Calculate token end seconds and current seconds
                            long currentSeconds = IOCommonHelpers.UnixTimeFromDate(DateTime.UtcNow);
                            long tokenEndSeconds = IOCommonHelpers.UnixTimeFromDate(userEntity.TokenDate.DateTime) + tokenLife;

							// Compare user token
                            if (currentSeconds < tokenEndSeconds && userEntity.UserToken.Equals(tokenData[1])) {
								// Dispose realm
								realm.Dispose();

								// Return is back office
                                return true;
                            }

							// Dispose realm
							realm.Dispose();

							// Return is not back office
							return false;
					    }

                        // Dispose realm
                        realm.Dispose();

						// Return is not back office
						return false;
                    }

					// Return is not back office
					return false;
                }

                // Return is not back office
                return false;
            }

            // Then return back office
            return true;
        }

        #endregion

    }
}
