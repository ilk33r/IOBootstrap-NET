using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using Microsoft.Extensions.Configuration;
using Realms;
using System;
using System.Linq;

namespace IOBootstrap.NET.WebApi.Authentication.ViewModels
{
    public abstract class IOAuthenticationViewModel : IOViewModel
    {

        #region Initialization Methods

        public IOAuthenticationViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<bool, string, DateTimeOffset> AuthenticateUser(string userName, string password) 
        {
			// Obtain realm instance 
			Realm realm = this.Database.GetRealmForThread();

			// Obtain user entity
			var userEntities = realm.All<IOUserEntity>()
                                    .Where((arg1) => arg1.UserName == userName.ToLower());

			// Check user finded
			if (userEntities.Count() > 0)
			{
				// Obtain user entity
				IOUserEntity user = userEntities.First();

				// Check user password is wrong
				if (!IOCommonHelpers.VerifyPassword(password, user.Password))
				{
					// Dispose realm
					realm.Dispose();

                    // Return response
                    return new Tuple<bool, string, DateTimeOffset>(false, null, DateTime.UtcNow);
				}

				// Generate token for user
				string userTokenString = IOCommonHelpers.GenerateRandomAlphaNumericString(32);

				// Create decrypted user token string
				string decryptedUserToken = String.Format("{0}-{1}", user.ID, userTokenString);

				// Convert key and iv to byte array
				byte[] key = Convert.FromBase64String(this.Configuration.GetValue<string>("IOEncryptionKey"));
				byte[] iv = Convert.FromBase64String(this.Configuration.GetValue<string>("IOEncryptionIV"));

				// Encode user token
				byte[] userTokenData = IOCommonHelpers.EncryptStringToBytes(decryptedUserToken, key, iv);

				// Base 64 encode user token data
				string userToken = Convert.ToBase64String(userTokenData);

				// Create token date
				DateTime tokenDate = DateTime.UtcNow;

				// Obtain token life from configuration
				int tokenLife = this.Configuration.GetValue<int>("IOTokenLife");

				// Begin write transaction
				Transaction realmTransaction = realm.BeginWrite();

				// Update entity properties
				user.UserToken = userTokenString;
				user.TokenDate = tokenDate;

				// Delete all entity
				realm.Add(user, true);

				// Write transaction
				realmTransaction.Commit();

				// Dispose realm
				realm.Dispose();

                // Return response
                return new Tuple<bool, string, DateTimeOffset>(true, userToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)));
			}

			// Dispose realm
			realm.Dispose();

			// Return response
			return new Tuple<bool, string, DateTimeOffset>(false, null, DateTime.UtcNow);
        }

        #endregion

    }
}
