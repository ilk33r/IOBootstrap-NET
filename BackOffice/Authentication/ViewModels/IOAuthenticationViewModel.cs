using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Authentication.ViewModels
{
    public abstract class IOAuthenticationViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOAuthenticationViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<string, DateTimeOffset, string, int> AuthenticateUser(string userName, string password) 
        {
            // Obtain user entity
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                Where = userName
            };
            IOMWUserResponseModel findedUserEntity = MWConnector.Get<IOMWUserResponseModel>(controller + "/" + "FindUserFromName", requestModel);
            MWConnector.HandleResponse(findedUserEntity, code => {
                // Return response
                throw new IOInvalidCredentialsException();
            });

			// Check user password is wrong
            if (!IOPasswordUtilities.VerifyPassword(password, findedUserEntity.Password))
			{
                // Return response
                throw new IOInvalidCredentialsException();
			}

			// Generate token for user
            string userTokenString = IORandomUtilities.GenerateGUIDString();

			// Create decrypted user token string
			string decryptedUserToken = String.Format("{0},{1}", findedUserEntity.ID, userTokenString);

			// Convert key and iv to byte array
			byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
			byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));

			// Base 64 encode user token data
            IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
			string userNewToken = aesUtilities.Encrypt(decryptedUserToken);

			// Create token date
			DateTime tokenDate = DateTime.UtcNow;

			// Obtain token life from configuration
			int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

			// Update entity properties
            IOMWUpdateTokenRequestModel updatedUser = new IOMWUpdateTokenRequestModel()
            {
                ID = findedUserEntity.ID,
                UserToken = userTokenString,
                TokenDate = tokenDate
            };
			IOResponseModel tokenResponse = MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateUserToken", updatedUser);
            MWConnector.HandleResponse(tokenResponse, code => {
                // Return response
                throw new IOInvalidCredentialsException();
            });

            // Return response
            return new Tuple<string, DateTimeOffset, string, int>(userNewToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)), findedUserEntity.UserName, findedUserEntity.UserRole);
        }

        public Tuple<DateTimeOffset, string, int> CheckToken(string token)
        {
            // Parse token data
            Tuple<string, int> tokenData = ParseToken(token);

            // Obtain user entity from database
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = tokenData.Item2
            };
            IOMWUserResponseModel findedUserEntity = MWConnector.Get<IOMWUserResponseModel>(controller + "/" + "FindUserById", requestModel);
            MWConnector.HandleResponse(findedUserEntity, code => {
                // Return response
                throw new IOInvalidCredentialsException();
            });

            // Obtain token life from configuration
            int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

            // Calculate token end seconds and current seconds
            long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
            long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUserEntity.TokenDate.DateTime) + tokenLife;

            // Compare user token
            if (findedUserEntity.UserToken != null && currentSeconds < tokenEndSeconds && findedUserEntity.UserToken.Equals(tokenData.Item1))
            {
                // Return status
                return new Tuple<DateTimeOffset, string, int>(findedUserEntity.TokenDate.DateTime, findedUserEntity.UserName, findedUserEntity.UserRole);
            }

            // Return status
            throw new IOInvalidCredentialsException();
        }

        #endregion

    }
}
