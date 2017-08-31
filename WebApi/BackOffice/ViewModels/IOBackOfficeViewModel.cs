using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.Common.Entities.AutoIncrements;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.Extensions.Configuration;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeViewModel : IOViewModel
    {

        #region Initialization Methods

        public IOBackOfficeViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public IOClientBackOfficeInfoModel CreateClient(string clientDescription) 
        {
			// Create a client entity
			IOClientsEntity clientEntity = new IOClientsEntity()
			{
                ID = IOAutoIncrementsEntity.IdForClass(this.Database, typeof(IOClientsEntity)),
				ClientId = IOCommonHelpers.GenerateRandomAlphaNumericString(32),
				ClientSecret = IOCommonHelpers.GenerateRandomAlphaNumericString(64),
				ClientDescription = clientDescription
			};

			// Write client to database
			this.Database.InsertEntity(clientEntity)
					 .Subscribe();

			// Create and return client info
			return new IOClientBackOfficeInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret);
        }

        public bool DeleteClient(int clientId) 
        {
			// Obtain realm instance 
			Realm realm = this.Database.GetRealmForMainThread();

			// Obtain client entity
			IOClientsEntity clientEntity = realm.Find<IOClientsEntity>(clientId);

			// Check client entity is not null
			if (clientEntity != null)
			{
				// Begin write transaction
				Transaction realmTransaction = realm.BeginWrite();

				// Delete all entity
				realm.Remove(clientEntity);

				// Write transaction
				realmTransaction.Commit();

				// Dispose realm
				realm.Dispose();

                // Then success
                return true;
			}

			// Dispose realm
			realm.Dispose();

            // Return delete operation is failed
            return false;
        }

		public List<IOClientBackOfficeInfoModel> GetClients()
		{
			// Create list for clients
			List<IOClientBackOfficeInfoModel> clientInfos = new List<IOClientBackOfficeInfoModel>();

			// Obtain realm 
			Realm realm = this.Database.GetRealmForMainThread();

			// Check real is not null
			if (realm != null)
			{
				// Obtain clients from realm
				var clients = realm.All<IOClientsEntity>();

				// Check clients is not null
				if (clients != null)
				{
					// Loop throught clients
					for (int i = 0; i < clients.Count(); i++)
					{
						// Obtain client entity
						IOClientsEntity client = clients.ElementAt(i);

						// Create back office info model
						IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.ID, client.ClientId, client.ClientSecret);

						// Add model to client info list
						clientInfos.Add(model);
					}
				}
			}

			// Dispose realm
			realm.Dispose();

			// Return clients
			return clientInfos;
		}

		public bool IsBackOffice()
		{
			// Check back office is open and token exists
			if (this.Configuration.GetValue<bool>("IOBackOfficeIsPublic"))
			{
				// Check authorization token exists
				if (this.Request.Headers.ContainsKey("X-IO-AUTHORIZATION-TOKEN"))
				{
					// Obtain request authorization value
					string requestAuthorization = this.Request.Headers["X-IO-AUTHORIZATION-TOKEN"];

					// Convert key and iv to byte array
					byte[] key = Convert.FromBase64String(this.Configuration.GetValue<string>("IOEncryptionKey"));
					byte[] iv = Convert.FromBase64String(this.Configuration.GetValue<string>("IOEncryptionIV"));

					// Obtain decrypted token value
					string decryptedToken = IOCommonHelpers.DecryptStringFromBytes(Convert.FromBase64String(requestAuthorization), key, iv);

					// Split user id and token value
					string[] tokenData = decryptedToken.Split('-');

					// Obtain user id from token data
					int userId = int.Parse(tokenData[0]);

					// Check token data is correct
					if (tokenData.Count() > 1)
					{
						// Obtain realm instance
						Realm realm = this.Database.GetRealmForThread();

						// Obtain user entity from database
						IOUserEntity userEntity = realm.Find<IOUserEntity>(userId);

						// Check user entity is not null
						if (userEntity != null)
						{
							// Obtain token life from configuration
							int tokenLife = this.Configuration.GetValue<int>("IOTokenLife");

							// Calculate token end seconds and current seconds
							long currentSeconds = IOCommonHelpers.UnixTimeFromDate(DateTime.UtcNow);
							long tokenEndSeconds = IOCommonHelpers.UnixTimeFromDate(userEntity.TokenDate.DateTime) + tokenLife;

							// Compare user token
							if (userEntity.UserToken != null && currentSeconds < tokenEndSeconds && userEntity.UserToken.Equals(tokenData[1]))
							{
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
