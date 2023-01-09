using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Common.Encryption;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOViewModel<TDBContext> : IIOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public string ClientId;
        public string ClientDescription;

        #endregion

        #region Properties

        public IConfiguration Configuration { get; set; }
        public IWebHostEnvironment Environment { get; set; }
        public ILogger<IOLoggerType> Logger { get; set; }
        public HttpRequest Request { get; set; }
        public TDBContext DatabaseContext { get; set; }

        #endregion

        #region Initialization Methods

        public IOViewModel()
        {
        }

		#endregion

		#region Helper Methods

        public virtual void CheckAuthorizationHeader()
		{
			// Check authorization header key exists
			if (Request.Headers.ContainsKey(IORequestHeaderConstants.Authorization))
			{
				// Obtain request authorization value
				string requestAuthorization = Request.Headers[IORequestHeaderConstants.Authorization];

				// Check authorization code is equal to configuration value
				if (requestAuthorization.Equals(Configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey)))
				{
					// Then authorization success
					return;
				}
			}

			throw new IOUnAuthorizeException();
		}

        public virtual void CheckClient()
		{
            // Obtain client info
            bool checkClientInfo = Configuration.GetValue<bool>(IOConfigurationConstants.CheckClientInfo);
            if (!checkClientInfo)
            {
                return;
            }

            // Obtain client ID and Secret
            string clientId = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientId)) ? (string)Request.Headers[IORequestHeaderConstants.ClientId] : "";
            string clientSecret = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientSecret)) ? (string)Request.Headers[IORequestHeaderConstants.ClientSecret] : "";

            // Find client
            var clientsEntity = DatabaseContext.Clients.Where((arg1) => arg1.ClientId.Equals(clientId));

			// Check finded client counts is greater than zero
			if (clientsEntity.Count() == 0)
			{
                // Then return invalid clients
			    throw new IOInvalidClientException();
			}

            // Obtain client
			IOClientsEntity client = clientsEntity.First();

			// Check client secret
            if (client.IsEnabled == 1 && client.ClientSecret.Equals(clientSecret))
			{
                // Obtain request counts
                long requestCount = client.RequestCount + 1;
                long maxRequestCount = client.MaxRequestCount;

                // Check request counts
                if (requestCount <= maxRequestCount)
                {
                    // Update request count
                    client.RequestCount = requestCount;

                    // Update properties
                    ClientId = clientId;
                    ClientDescription = client.ClientDescription;

                    // Update client 
                    DatabaseContext.Update(client);
                    DatabaseContext.SaveChanges();
                }
			}
		}

        public virtual int GetUserRole()
        {
            return (int)UserRoles.SuperAdmin;
        }

        #endregion

        #region Encryption Decryption

        public virtual string DecryptString(string encryptedString)
        {
            if (encryptedString == null) {
                return "";
            }

            IOAESUtilities aesUtility = GetAesUtility();
            return aesUtility.Decrypt(encryptedString);
        }

        public virtual string EncryptString(string plainString)
        {
            IOAESUtilities aesUtility = GetAesUtility();
            return aesUtility.Encrypt(plainString);
        }

        public virtual IOAESUtilities GetAesUtility()
        {
            string symmetricIVString = Request.Headers[IORequestHeaderConstants.SymmetricIV];
            string symmetricKeyString = Request.Headers[IORequestHeaderConstants.SymmetricKey];
            
            try {
                byte[] encryptedSymmetricIV = Convert.FromBase64String(symmetricIVString);
                byte[] symmetricIV = IOEncryptionUtilities.DecryptString(encryptedSymmetricIV);
            
                byte[] encryptedSymmetricKey = Convert.FromBase64String(symmetricKeyString);
                byte[] symmetricKey = IOEncryptionUtilities.DecryptString(encryptedSymmetricKey);

                if (symmetricIV == null || symmetricKey == null)
                {
                    throw new IOInvalidKeyIDException();
                }

                return new IOAESUtilities(symmetricKey, symmetricIV);
            } 
            catch (Exception e)
            {
                Logger.LogDebug("{0}", e.StackTrace);
                throw new IOInvalidKeyIDException();
            }
        }

        #endregion

        #region Configuration

        public virtual IOConfigurationModel GetDBConfig(string configKey)
        {
            string cacheKey = IOCacheKeys.ConfigurationCacheKey + configKey;
            IOCacheObject cachedObject = IOCache.GetCachedObject(cacheKey);
            if (cachedObject != null)
            {
                IOConfigurationModel configurationModel = (IOConfigurationModel)cachedObject.Value;
                return configurationModel;
            }

            IOConfigurationModel configuration = DatabaseContext.Configurations
                                                                            .Select(c => new IOConfigurationModel()
                                                                            {
                                                                                ConfigKey = c.ConfigKey,
                                                                                ConfigIntValue = c.ConfigIntValue,
                                                                                ConfigStringValue = c.ConfigStringValue
                                                                            })
                                                                            .Where(config => config.ConfigKey.Equals(configKey))
                                                                            .FirstOrDefault();

            
            if (configuration != null)
            {
                cachedObject = new IOCacheObject(cacheKey, configuration, 0);
                IOCache.CacheObject(cachedObject);

                return configuration;
            }

            return null;
        }

        #endregion
    }
}
