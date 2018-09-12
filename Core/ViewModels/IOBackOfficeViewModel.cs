using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public IOUserEntity userEntity;

        #endregion

        #region Initialization Methods

        public IOBackOfficeViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public IOClientBackOfficeInfoModel CreateClient(string clientDescription, int maxRequestCount) 
        {
			// Create a client entity
			IOClientsEntity clientEntity = new IOClientsEntity()
			{
				ClientId = IOCommonHelpers.GenerateRandomAlphaNumericString(32),
				ClientSecret = IOCommonHelpers.GenerateRandomAlphaNumericString(64),
				ClientDescription = clientDescription,
                IsEnabled = 1,
                RequestCount = 0,
                MaxRequestCount = maxRequestCount
			};

            // Write client to database
            _databaseContext.Clients.Add(clientEntity);
            _databaseContext.SaveChanges();

			// Create and return client info
            return new IOClientBackOfficeInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret, clientEntity.ClientDescription, 1, 0, maxRequestCount);
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
                _databaseContext.SaveChanges();

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
                    IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.ID, 
                                                                                        client.ClientId, 
                                                                                        client.ClientSecret, 
                                                                                        client.ClientDescription, 
                                                                                        client.IsEnabled,
                                                                                        client.RequestCount,
                                                                                        client.MaxRequestCount);

					// Add model to client info list
					clientInfos.Add(model);
				}
			}

			// Return clients
			return clientInfos;
		}

        public bool UpdateClient(int id, string description, int isEnabled, int requestCount, int maxRequestCount)
        {
            // Obtain client entity
            var clientEntities = _databaseContext.Clients.Where((arg1) => arg1.ID == id);
           
            // Check client finded
            if (clientEntities.Count() > 0)
            {
                // Obtain user entity
                IOClientsEntity client = clientEntities.First();

                // Update client properties
                client.ClientDescription = description;
                client.IsEnabled = isEnabled;
                client.RequestCount = requestCount;
                client.MaxRequestCount = maxRequestCount;

                // Update client
                _databaseContext.Update(client);
                _databaseContext.SaveChanges();

                // Return response
                return true;
            }

            // Return response
            return false;
        }

        public bool IsBackOffice()
		{
			// Check back office is not open and token exists
            if (!_configuration.GetValue<bool>("IOBackOfficeIsPublic") && _request.Headers.ContainsKey("X-IO-AUTHORIZATION-TOKEN"))
			{
                // Obtain token
                string token = _request.Headers["X-IO-AUTHORIZATION-TOKEN"];

                // Parse token
                Tuple<string, int> tokenData = this.parseToken(token);

                // Return back office status
                return this.checkBackofficeTokenIsValid(tokenData.Item1, tokenData.Item2);
			}

			// Then return back office
			return true;
		}

        private bool checkBackofficeTokenIsValid(string tokenData, int userId) {
            // Check token data is correct
            if (tokenData.Count() > 1)
            {
                // Obtain user entity from database
                this.userEntity = _databaseContext.Users.Find(userId);

                // Check user entity is not null
                if (userEntity != null)
                {
                    // Obtain token life from configuration
                    int tokenLife = _configuration.GetValue<int>("IOTokenLife");

                    // Calculate token end seconds and current seconds
                    long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
                    long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(userEntity.TokenDate.DateTime) + tokenLife;

                    // Compare user token
                    if (userEntity.UserToken != null && currentSeconds < tokenEndSeconds && userEntity.UserToken.Equals(tokenData))
                    {
                        // Return is back office
                        return true;
                    }
                }
            }

            // Return is not back office
            return false;
        }

        public Tuple<string, int> parseToken(string token) 
        {
            // Convert key and iv to byte array
            byte[] key = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionKey"));
            byte[] iv = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionIV"));

            // Obtain decrypted token value
            string decryptedToken = IOPasswordUtilities.DecryptStringFromBytes(Convert.FromBase64String(token), key, iv);

            // Split user id and token value
            string[] tokenData = decryptedToken.Split('-');

            // Obtain user id from token data
            int userId = int.Parse(tokenData[0]);

            return new Tuple<string, int>(tokenData[1], userId);
        }

        #endregion

    }
}
