using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public IOUserEntity UserEntity;

        #endregion

        #region Initialization Methods

        public IOBackOfficeViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual IOClientInfoModel CreateClient(string clientDescription, long maxRequestCount)
        {
            // Create a client entity
            IOClientsEntity clientEntity = new IOClientsEntity()
            {
                ClientId = IORandomUtilities.GenerateGUIDString(),
                ClientSecret = IORandomUtilities.GenerateGUIDString(),
                ClientDescription = clientDescription,
                IsEnabled = 1,
                RequestCount = 0,
                MaxRequestCount = maxRequestCount
            };

            // Write client to database
            DatabaseContext.Clients.Add(clientEntity);
            DatabaseContext.SaveChanges();

            // Create and return client info
            return new IOClientInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret, clientEntity.ClientDescription, 1, 0, maxRequestCount);
        }

        public void DeleteClient(int clientId)
        {
            // Obtain client entity
            IOClientsEntity clientEntity = DatabaseContext.Clients.Find(clientId);

            // Check client entity is not null
            if (clientEntity == null)
            {
                throw new IOInvalidClientException("Client not found.");
            }

            // Delete all entity
            DatabaseContext.Remove(clientEntity);
            DatabaseContext.SaveChanges();
        }

        public List<IOClientInfoModel> GetClients()
        {
            // Create list for clients
            List<IOClientInfoModel> clientInfos = new List<IOClientInfoModel>();

            // Obtain clients from realm
            var clients = DatabaseContext.Clients;

            // Check clients is not null
            if (clients != null)
            {
                List<IOClientsEntity> clientsEntity = clients.ToList();
                clientInfos = clientsEntity.ConvertAll(client =>
                {
                    // Create back office info model
                    return new IOClientInfoModel(client.ID,
                                                client.ClientId,
                                                client.ClientSecret,
                                                client.ClientDescription,
                                                client.IsEnabled,
                                                client.RequestCount,
                                                client.MaxRequestCount);
                });
            }

            // Return clients
            return clientInfos;
        }

        public void UpdateClient(int id, string description, int isEnabled, long requestCount, long maxRequestCount)
        {
            // Obtain client entity
            var clientEntities = DatabaseContext.Clients.Where(arg1 => arg1.ID == id);

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
                DatabaseContext.Update(client);
                DatabaseContext.SaveChanges();

                // Return response
                return;
            }

            // Return response
            throw new IOInvalidClientException("Client not found.");
        }

        public bool IsBackOffice()
        {
            // Check back office is not open and token exists
            if (!Configuration.GetValue<bool>(IOConfigurationConstants.BackOfficeIsPublic) && Request.Headers.ContainsKey(IORequestHeaderConstants.AuthorizationToken))
            {
                // Obtain token
                string token = Request.Headers[IORequestHeaderConstants.AuthorizationToken];

                // Parse token
                Tuple<string, int> tokenData = ParseToken(token);

                // Return back office status
                return CheckBackofficeTokenIsValid(tokenData.Item1, tokenData.Item2);
            }

            // Then return back office
            return true;
        }

        private bool CheckBackofficeTokenIsValid(string tokenData, int userId)
        {
            // Check token data is correct
            if (tokenData.Count() > 1)
            {
                // Obtain user entity from database
                UserEntity = DatabaseContext.Users.Find(userId);

                // Check user entity is not null
                if (UserEntity != null)
                {
                    // Obtain token life from configuration
                    int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

                    // Calculate token end seconds and current seconds
                    long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
                    long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(UserEntity.TokenDate.DateTime) + tokenLife;

                    // Compare user token
                    if (UserEntity.UserToken != null && currentSeconds < tokenEndSeconds && UserEntity.UserToken.Equals(tokenData))
                    {
                        // Return is back office
                        return true;
                    }
                }
            }

            // Return is not back office
            return false;
        }

        public Tuple<string, int> ParseToken(string token)
        {
            // Convert key and iv to byte array
            byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
            byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));

            IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
            try
            {
                // Obtain decrypted token value
                string decryptedToken = aesUtilities.Decrypt(token);

                // Split user id and token value
                string[] tokenData = decryptedToken.Split(',');

                // Obtain user id from token data
                int userId = int.Parse(tokenData[0]);

                return new Tuple<string, int>(tokenData[1], userId);
            }
            catch (Exception e)
            {
                Logger.LogDebug(e.StackTrace);
                return new Tuple<string, int>("", 0);
            }
        }

        #endregion

        #region Helper Methods

        public override int GetUserRole()
        {
            IOUserEntity userEntity = UserEntity;

            // Check user exists
            if (userEntity != null)
            {
                // Return role
                return userEntity.UserRole;
            }

            if (Configuration.GetValue<bool>(IOConfigurationConstants.BackOfficeIsPublic))
            {
                return (int)UserRoles.SuperAdmin;
            }

            return (int)UserRoles.AnonmyMouse;
        }

        #endregion

    }
}
