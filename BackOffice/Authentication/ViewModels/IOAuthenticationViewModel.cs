﻿using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.BackOffice.Authentication.ViewModels
{
    public abstract class IOAuthenticationViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
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
            IOUserEntity user = IOUserEntity.FindUserFromName(DatabaseContext.Users, userName);

			// Check user finded
            if (user == null)
            {
                // Return response
                throw new IOInvalidCredentialsException();
            }

			// Check user password is wrong
            if (!IOPasswordUtilities.VerifyPassword(password, user.Password))
			{
                // Return response
                throw new IOInvalidCredentialsException();
			}

			// Generate token for user
            string userTokenString = IORandomUtilities.GenerateGUIDString();

			// Create decrypted user token string
			string decryptedUserToken = String.Format("{0},{1}", user.ID, userTokenString);

			// Convert key and iv to byte array
			byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey));
			byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV));

			// Base 64 encode user token data
            IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
			string userToken = aesUtilities.Encrypt(decryptedUserToken);

			// Create token date
			DateTime tokenDate = DateTime.UtcNow;

			// Obtain token life from configuration
			int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

			// Update entity properties
			user.UserToken = userTokenString;
			user.TokenDate = tokenDate;

            // Delete all entity
            DatabaseContext.Update(user);
            DatabaseContext.SaveChanges();

            // Return response
            return new Tuple<string, DateTimeOffset, string, int>(userToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)), user.UserName, user.UserRole);
        }

        public Tuple<DateTimeOffset, string, int> CheckToken(string token)
        {
            // Parse token data
            Tuple<string, int> tokenData = ParseToken(token);

            // Obtain user entity from database
            IOUserEntity findedUserEntity = DatabaseContext.Users.Find(tokenData.Item2);

            // Check user entity is not null
            if (findedUserEntity == null)
            {
                // Return status
                throw new IOInvalidCredentialsException();
            }

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
