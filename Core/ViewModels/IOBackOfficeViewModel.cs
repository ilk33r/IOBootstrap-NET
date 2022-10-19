using System;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOBackOfficeViewModel : IOViewModel
    {

        #region Publics

        public IOMWUserResponseModel UserModel;

        #endregion

        #region Initialization Methods

        public IOBackOfficeViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual IOClientInfoModel CreateClient(IOClientAddRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IOMWObjectResponseModel<IOClientInfoModel> client = MWConnector.Get<IOMWObjectResponseModel<IOClientInfoModel>>(controller + "/" + "AddClient", requestModel);
            MWConnector.HandleResponse(client, code => {
                throw new IOMWConnectionException();
            });

            // Create and return client info
            return client.Item;
        }

        public void DeleteClient(IOClientDeleteRequestModel requestModel)
        {
            // Obtain client entity
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IOResponseModel deletedClient = MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteClient", requestModel);
            MWConnector.HandleResponse(deletedClient, code => {
                // Return response
                throw new IOInvalidClientException("Client not found.");
            });
        }
        
        public IList<IOClientInfoModel> GetClients()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IOMWListResponseModel<IOClientInfoModel> clients = MWConnector.Get<IOMWListResponseModel<IOClientInfoModel>>(controller + "/" + "ListClients", new IOMWFindRequestModel());
            if (MWConnector.HandleResponse(clients, code => {}))
            {
                // Return clients
                return clients.Items;
            }
            
            return new List<IOClientInfoModel>();
        }

        public void UpdateClient(IOClientUpdateRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IOResponseModel updateClient = MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateClient", requestModel);
            MWConnector.HandleResponse(updateClient, code => {
                // Return response
                throw new IOInvalidClientException("Client not found.");
            });
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
                // Obtain user entity from database
                string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
                IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
                {
                    ID = userId
                };
                IOMWUserResponseModel findedUserEntity;
                string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, userId);
                IOCacheObject userCache = IOCache.GetCachedObject(cacheKey);
                if (userCache != null)
                {
                    findedUserEntity = (IOMWUserResponseModel)userCache.Value;
                }
                else
                {
                    findedUserEntity = MWConnector.Get<IOMWUserResponseModel>(controller + "/" + "FindUserById", requestModel);
                    if (!MWConnector.HandleResponse(findedUserEntity, code => {}))
                    {
                        // Return is not back office
                        return false;
                    }
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
