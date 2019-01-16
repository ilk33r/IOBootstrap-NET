﻿using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.WebApi.Authentication.ViewModels
{
    public abstract class IOAuthenticationViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOAuthenticationViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<bool, string, DateTimeOffset, string, int> AuthenticateUser(string userName, string password) 
        {
            // Obtain user entity
            IOUserEntity user = IOUserEntity.FindUserFromName(_databaseContext.Users, userName);

			// Check user finded
            if (user != null)
			{
				// Check user password is wrong
                if (!IOPasswordUtilities.VerifyPassword(password, user.Password))
				{
                    // Return response
                    return new Tuple<bool, string, DateTimeOffset, string, int>(false, null, DateTime.UtcNow, null, 0);
				}

				// Generate token for user
                string userTokenString = IORandomUtilities.GenerateRandomAlphaNumericString(32);

				// Create decrypted user token string
				string decryptedUserToken = String.Format("{0}-{1}", user.ID, userTokenString);

				// Convert key and iv to byte array
				byte[] key = Convert.FromBase64String(_configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
				byte[] iv = Convert.FromBase64String(_configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));

				// Encode user token
                byte[] userTokenData = IOPasswordUtilities.EncryptStringToBytes(decryptedUserToken, key, iv);

				// Base 64 encode user token data
				string userToken = Convert.ToBase64String(userTokenData);

				// Create token date
				DateTime tokenDate = DateTime.UtcNow;

				// Obtain token life from configuration
				int tokenLife = _configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

				// Update entity properties
				user.UserToken = userTokenString;
				user.TokenDate = tokenDate;

                // Delete all entity
                _databaseContext.Update(user);
                _databaseContext.SaveChanges();

                // Return response
                return new Tuple<bool, string, DateTimeOffset, string, int>(true, userToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)), user.UserName, user.UserRole);
			}

			// Return response
            return new Tuple<bool, string, DateTimeOffset, string, int>(false, null, DateTime.UtcNow, null, 0);
        }

        public Tuple<bool, DateTimeOffset, string, int> CheckToken(string token)
        {
            // Parse token data
            Tuple<string, int> tokenData = this.parseToken(token);

            // Obtain user entity from database
            IOUserEntity findedUserEntity = _databaseContext.Users.Find(tokenData.Item2);

            // Check user entity is not null
            if (findedUserEntity != null)
            {
                // Obtain token life from configuration
                int tokenLife = this._configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

                // Calculate token end seconds and current seconds
                long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
                long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUserEntity.TokenDate.DateTime) + tokenLife;

                // Compare user token
                if (findedUserEntity.UserToken != null && currentSeconds < tokenEndSeconds && findedUserEntity.UserToken.Equals(tokenData.Item1))
                {
                    // Return status
                    return new Tuple<bool, DateTimeOffset, string, int>(true, findedUserEntity.TokenDate.DateTime, findedUserEntity.UserName, findedUserEntity.UserRole);
                }
            }

            // Return status
            return new Tuple<bool, DateTimeOffset, string, int>(false, DateTime.UtcNow, null, 0);
        }

        #endregion

    }
}
