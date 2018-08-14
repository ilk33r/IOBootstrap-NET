using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

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

        public Tuple<bool, string, DateTimeOffset> AuthenticateUser(string userName, string password) 
        {
			// Obtain user entity
            var userEntities = _databaseContext.Users.Where((arg1) => arg1.UserName == userName.ToLower());

			// Check user finded
			if (userEntities.Count() > 0)
			{
				// Obtain user entity
				IOUserEntity user = userEntities.First();

				// Check user password is wrong
				if (!IOCommonHelpers.VerifyPassword(password, user.Password))
				{
                    // Return response
                    return new Tuple<bool, string, DateTimeOffset>(false, null, DateTime.UtcNow);
				}

				// Generate token for user
				string userTokenString = IOCommonHelpers.GenerateRandomAlphaNumericString(32);

				// Create decrypted user token string
				string decryptedUserToken = String.Format("{0}-{1}", user.ID, userTokenString);

				// Convert key and iv to byte array
				byte[] key = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionKey"));
				byte[] iv = Convert.FromBase64String(_configuration.GetValue<string>("IOEncryptionIV"));

				// Encode user token
				byte[] userTokenData = IOCommonHelpers.EncryptStringToBytes(decryptedUserToken, key, iv);

				// Base 64 encode user token data
				string userToken = Convert.ToBase64String(userTokenData);

				// Create token date
				DateTime tokenDate = DateTime.UtcNow;

				// Obtain token life from configuration
				int tokenLife = _configuration.GetValue<int>("IOTokenLife");

				// Update entity properties
				user.UserToken = userTokenString;
				user.TokenDate = tokenDate;

                // Delete all entity
                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();

                // Return response
                return new Tuple<bool, string, DateTimeOffset>(true, userToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)));
			}

			// Return response
			return new Tuple<bool, string, DateTimeOffset>(false, null, DateTime.UtcNow);
        }

        public Tuple<bool, DateTimeOffset> CheckToken(string token)
        {
            // Parse token data
            Tuple<string, int> tokenData = this.parseToken(token);

            // Obtain user entity from database
            IOUserEntity userEntity = _databaseContext.Users.Find(tokenData.Item2);

            // Check user entity is not null
            if (userEntity != null)
            {
                // Obtain token life from configuration
                int tokenLife = this._configuration.GetValue<int>("IOTokenLife");

                // Calculate token end seconds and current seconds
                long currentSeconds = IOCommonHelpers.UnixTimeFromDate(DateTime.UtcNow);
                long tokenEndSeconds = IOCommonHelpers.UnixTimeFromDate(userEntity.TokenDate.DateTime) + tokenLife;

                // Compare user token
                if (userEntity.UserToken != null && currentSeconds < tokenEndSeconds && userEntity.UserToken.Equals(tokenData.Item1))
                {
                    // Return status
                    return new Tuple<bool, DateTimeOffset>(true, userEntity.TokenDate.DateTime);
                }
            }

            // Return status
            return new Tuple<bool, DateTimeOffset>(false, DateTime.UtcNow);
        }

        #endregion

    }
}
