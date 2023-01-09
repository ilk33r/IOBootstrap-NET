using System;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext>, IIOBackOfficeViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public IOUserInfoModel UserModel  { get; set; }

        #endregion

        #region Initialization Methods

        public IOBackOfficeViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual IOClientInfoModel CreateClient(IOClientAddRequestModel requestModel)
        {
            // Create a client entity
            IOClientsEntity clientEntity = new IOClientsEntity()
            {
                ClientId = IORandomUtilities.GenerateGUIDString(),
                ClientSecret = IORandomUtilities.GenerateGUIDString(),
                ClientDescription = requestModel.ClientDescription,
                IsEnabled = 1,
                RequestCount = 0,
                MaxRequestCount = requestModel.RequestCount
            };

            // Write client to database
            DatabaseContext.Clients.Add(clientEntity);
            DatabaseContext.SaveChanges();

            // Create and return client info
            return new IOClientInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret, clientEntity.ClientDescription, 1, 0, clientEntity.MaxRequestCount);
        }

        public void DeleteClient(IOClientDeleteRequestModel requestModel)
        {
            IOClientsEntity clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

            // Check client entity is not null
            if (clientEntity == null)
            {
                throw new IOInvalidClientException("Client not found.");
            }

            // Delete all entity
            DatabaseContext.Remove(clientEntity);
            DatabaseContext.SaveChanges();
        }
        
        public IList<IOClientInfoModel> GetClients()
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

        public void UpdateClient(IOClientUpdateRequestModel requestModel)
        {
            // Obtain client entity
            IOClientsEntity clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

            // Check client finded
            if (clientEntity != null)
            {
                // Update client properties
                clientEntity.ClientDescription = requestModel.ClientDescription;
                clientEntity.IsEnabled = requestModel.IsEnabled;
                clientEntity.RequestCount = requestModel.RequestCount;
                clientEntity.MaxRequestCount = requestModel.MaxRequestCount;

                // Update client
                DatabaseContext.Update(clientEntity);
                DatabaseContext.SaveChanges();

                // Return response
                return;
            }

            // Return response
            throw new IOInvalidClientException("Client not found.");
        }

        public virtual bool IsBackOffice()
        {
            // Check back office is not open and token exists
            if (Request.Headers.ContainsKey(IORequestHeaderConstants.AuthorizationToken))
            {
                // Obtain token
                string token = Request.Headers[IORequestHeaderConstants.AuthorizationToken];

                // Parse token
                Tuple<string, int> tokenData = ParseToken(token);

                // Return back office status
                return CheckBackofficeTokenIsValid(tokenData.Item1, tokenData.Item2);
            }

            // Then return back office
            return false;
        }

        private bool CheckBackofficeTokenIsValid(string tokenData, int userId)
        {
            // Check token data is correct
            if (tokenData.Count() > 1)
            {
                IOUserInfoModel findedUserEntity;
                
                // Obtain user entity from database
                string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, userId);
                IOCacheObject userCache = IOCache.GetCachedObject(cacheKey);

                if (userCache != null)
                {
                    findedUserEntity = (IOUserInfoModel)userCache.Value;
                }
                else
                {
                    findedUserEntity = DatabaseContext.Users
                                                        .Select(u => new IOUserInfoModel()
                                                        {
                                                            ID = u.ID,
                                                            Password = u.Password,
                                                            UserName = u.UserName,
                                                            UserRole = u.UserRole,
                                                            UserToken = u.UserToken,
                                                            TokenDate = u.TokenDate
                                                        })
                                                        .Where(u => u.ID == userId)
                                                        .FirstOrDefault();
                }
                
                // Check user entity is not null
                if (findedUserEntity == null)
                {
                    // Return is not back office
                    return false;
                }
                 
                // Obtain token life from configuration
                int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

                // Calculate token end seconds and current seconds
                long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
                long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUserEntity.TokenDate.DateTime) + tokenLife;

                // Compare user token
                if (findedUserEntity.UserToken != null && currentSeconds < tokenEndSeconds && findedUserEntity.UserToken.Equals(tokenData))
                {
                    // Return is back office
                    UserModel = findedUserEntity;
                    if (userCache == null)
                    {
                        userCache = new IOCacheObject(cacheKey, findedUserEntity, 60);
                        IOCache.CacheObject(userCache);
                    }
                    return true;
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
            // Check user exists
            if (UserModel != null)
            {
                // Return role
                return UserModel.UserRole;
            }

            return (int)UserRoles.AnonmyMouse;
        }
        
        #endregion

    }
}
