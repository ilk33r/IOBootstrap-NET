﻿using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
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
				ClientId = IOCommonHelpers.GenerateRandomAlphaNumericString(32),
				ClientSecret = IOCommonHelpers.GenerateRandomAlphaNumericString(64),
				ClientDescription = clientDescription
			};

            // Write client to database
            _databaseContext.Clients.Add(clientEntity);
            _databaseContext.SaveChanges();

			// Create and return client info
			return new IOClientBackOfficeInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret);
        }

        public bool DeleteClient(int clientId) 
        {
            // Obtain client entity
            IOClientsEntity clientEntity = _databaseContext.Clients.Find(clientId);

			// Check client entity is not null
			if (clientEntity != null)
			{
                // Delete all entity
                _databaseContext.Remove(clientEntity);
                _databaseContext.SaveChangesAsync();

                // Then success
                return true;
			}

            // Return delete operation is failed
            return false;
        }

		public List<IOClientBackOfficeInfoModel> GetClients()
		{
			// Create list for clients
			List<IOClientBackOfficeInfoModel> clientInfos = new List<IOClientBackOfficeInfoModel>();

            // Obtain clients from realm
            var clients = _databaseContext.Clients;

			// Check clients is not null
			if (clients != null)
			{
				// Loop throught clients
				for (int i = 0; i < clients.Count(); i++)
				{
					// Obtain client entity
					IOClientsEntity client = clients.Skip(i).First();

					// Create back office info model
					IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.ID, client.ClientId, client.ClientSecret);

					// Add model to client info list
					clientInfos.Add(model);
				}
			}

			// Return clients
			return clientInfos;
		}

		public bool IsBackOffice()
		{
			// Check back office is open and token exists
			if (_configuration.GetValue<bool>("IOBackOfficeIsPublic"))
			{
				// Check authorization token exists
				if (_request.Headers.ContainsKey("X-IO-AUTHORIZATION-TOKEN"))
				{
					// Obtain request authorization value
					string requestAuthorization = _request.Headers["X-IO-AUTHORIZATION-TOKEN"];

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
					if (tokenData.Count() > 1)
					{
                        // Obtain user entity from database
                        IOUserEntity userEntity = _databaseContext.Users.Find(userId);

						// Check user entity is not null
						if (userEntity != null)
						{
							// Obtain token life from configuration
							int tokenLife = _configuration.GetValue<int>("IOTokenLife");

							// Calculate token end seconds and current seconds
							long currentSeconds = IOCommonHelpers.UnixTimeFromDate(DateTime.UtcNow);
							long tokenEndSeconds = IOCommonHelpers.UnixTimeFromDate(userEntity.TokenDate.DateTime) + tokenLife;

							// Compare user token
							if (userEntity.UserToken != null && currentSeconds < tokenEndSeconds && userEntity.UserToken.Equals(tokenData[1]))
							{
								// Return is back office
								return true;
							}

							// Return is not back office
							return false;
						}

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