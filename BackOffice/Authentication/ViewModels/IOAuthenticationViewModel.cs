using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Authentication.ViewModels
{
    public abstract class IOAuthenticationViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOAuthenticationViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    {

        #region Initialization Methods

        public IOAuthenticationViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual Tuple<string, DateTimeOffset, string, int> AuthenticateUser(string userName, string password) 
        {
            IOUserEntity findedUser = DatabaseContext.Users
                                                        .Where(u => u.UserName.Equals(userName))
                                                        .FirstOrDefault();

            if (findedUser == null)
            {
                // Return response
                throw new IOInvalidCredentialsException();
            }

			// Check user password is wrong
            if (!IOPasswordUtilities.VerifyPassword(password, findedUser.Password))
			{
                // Return response
                throw new IOInvalidCredentialsException();
			}

			// Generate token for user
            string userTokenString = IORandomUtilities.GenerateGUIDString();

			// Create decrypted user token string
			string decryptedUserToken = String.Format("{0},{1}", findedUser.ID, userTokenString);

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
            findedUser.UserToken = userTokenString;
            findedUser.TokenDate = tokenDate;
            
            DatabaseContext.Update(findedUser);
            DatabaseContext.SaveChanges();

            // Return response
            return new Tuple<string, DateTimeOffset, string, int>(userNewToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)), findedUser.UserName, findedUser.UserRole);
        }

        public virtual Tuple<DateTimeOffset, string, int> CheckToken(string token)
        {
            // Parse token data
            Tuple<string, int> tokenData = ParseToken(token);

            IOUserInfoModel findedUser = DatabaseContext.Users
                                                        .Select(u => new IOUserInfoModel()
                                                        {
                                                            ID = u.ID,
                                                            Password = u.Password,
                                                            UserName = u.UserName,
                                                            UserRole = u.UserRole,
                                                            UserToken = u.UserToken,
                                                            TokenDate = u.TokenDate
                                                        })
                                                        .Where(u => u.ID == tokenData.Item2)
                                                        .FirstOrDefault();

            if (findedUser == null)
            {
                throw new IOInvalidCredentialsException();
            }

            // Obtain token life from configuration
            int tokenLife = Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

            // Calculate token end seconds and current seconds
            long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
            long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUser.TokenDate.DateTime) + tokenLife;

            // Compare user token
            if (findedUser.UserToken != null && currentSeconds < tokenEndSeconds && findedUser.UserToken.Equals(tokenData.Item1))
            {
                // Return status
                return new Tuple<DateTimeOffset, string, int>(findedUser.TokenDate.DateTime, findedUser.UserName, findedUser.UserRole);
            }

            // Return status
            throw new IOInvalidCredentialsException();
        }

        #endregion

    }
}
