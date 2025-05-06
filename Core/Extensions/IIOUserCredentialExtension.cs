using System.Threading.Tasks;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOUserCredentialExtension
{
    public static bool CheckHasUserTokenAndIsValid<TDBContext>(this IIOUserCredential<TDBContext> input)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Check back office is not open and token exists
        if (input.Request.Headers.ContainsKey(IORequestHeaderConstants.AuthorizationToken))
        {
            // Obtain token
            string token = input.Request.Headers[IORequestHeaderConstants.AuthorizationToken]!;

            // Parse token
            Tuple<string, int> tokenData = input.ParseUserToken(token);

            // Return back office status
            return input.CheckUserTokenIsValid(tokenData.Item1, tokenData.Item2);
        }

        // Then return back office
        return false;
    }

    public static bool CheckUserTokenIsValid<TDBContext>(this IIOUserCredential<TDBContext> input, string tokenData, int userId)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Check token data is correct
        if (tokenData.Count() > 1)
        {
            IOUserInfoModel? findedUserEntity;

            // Obtain user entity from database
            string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, userId);
            IOCacheObject? userCache = IOCache.GetCachedObject(cacheKey);

            if (userCache != null)
            {
                findedUserEntity = (IOUserInfoModel)userCache.Value;
            }
            else
            {
                findedUserEntity = input.DatabaseContext.Users
                                                    .Select(u => new IOUserInfoModel()
                                                    {
                                                        ID = u.ID,
                                                        Password = u.Password,
                                                        UserName = u.UserName,
                                                        UserRole = u.UserRole,
                                                        UserToken = u.UserToken,
                                                        TokenDate = u.TokenDate,
                                                        PasswordExpireDate = u.PasswordExpireDate
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
            int tokenLife = input.Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

            // Calculate token end seconds and current seconds
            long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
            long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUserEntity.TokenDate.DateTime) + tokenLife;

            // Compare user token
            if (findedUserEntity.UserToken != null && currentSeconds < tokenEndSeconds && findedUserEntity.UserToken.Equals(tokenData))
            {
                // Return is back office
                input.UserModel = findedUserEntity;
                if (userCache == null)
                {
                    userCache = new IOCacheObject(cacheKey, findedUserEntity, 60);
                    IOCache.CacheObject(userCache);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        // Return is not back office
        return false;
    }

    public static Tuple<string, int> ParseUserToken<TDBContext>(this IIOUserCredential<TDBContext> input, string token)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        try
        {
            // Obtain decrypted token value
            string decryptedToken = aesUtilities.Decrypt(Convert.FromBase64String(token));

            // Split user id and token value
            string[] tokenData = decryptedToken.Split(',');

            // Obtain user id from token data
            int userId = int.Parse(tokenData[0]);

            return new Tuple<string, int>(tokenData[1], userId);
        }
        catch (Exception e)
        {
            input.Logger.LogDebug(e.StackTrace);
            return new Tuple<string, int>("", 0);
        }
    }

    public static async Task ChangeUserPassword<TDBContext>(this IIOUserCredential<TDBContext> input, string oldPassword, string newPassword)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        if (input.UserModel == null)
        {
            // Return user exists response
            throw new IOUserNotFoundException();
        }

        // Obtain decrypted passwords
        string decryptedOldPassword = input.DecryptString(oldPassword);
        string decryptedNewPassword = input.DecryptString(newPassword);

        // Obtain current user
        IOUserEntity? currentUser = input.DatabaseContext.Users
                                                    .Where(u => u.ID == input.UserModel.ID)
                                                    .FirstOrDefault();

        if (currentUser == null)
        {
            // Return user exists response
            throw new IOUserNotFoundException();
        }

        // Check user old password is valid
        using (var passwordUtilities = new IOPasswordUtilities())
        {
            await passwordUtilities.VerifyPassword(decryptedOldPassword, currentUser.Password ?? "", verified =>
            {
                // Check verify status
                if (!verified)
                {
                    // Return response
                    throw new IOInvalidPasswordException();
                }
            });

            // Update user password properties
            await passwordUtilities.HashPassword(decryptedNewPassword, hashed =>
            {
                currentUser.Password = hashed;
            });
        }

        currentUser.PasswordExpireDate = DateTimeOffset.UtcNow.AddMonths(6);
        currentUser.UserToken = null;

        // Update user password
        input.DatabaseContext.Update(currentUser);
        input.DatabaseContext.SaveChanges();
    }

    public static void LogoutUser<TDBContext>(this IIOUserCredential<TDBContext> input, string userName)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Decrypt user name
        string decryptedUserName = input.DecryptString(userName);

        // Validate user name
        if (!(input.UserModel?.UserName?.Equals(decryptedUserName) ?? false))
        {
            throw new IOInvalidCredentialsException();
        }

        IOUserEntity? findedUser = input.DatabaseContext.Users
                                                    .Where(u => u.ID == input.UserModel.ID)
                                                    .FirstOrDefault();

        if (findedUser == null)
        {
            // Return response
            throw new IOInvalidCredentialsException();
        }

        // Update entity properties
        findedUser.UserToken = null;

        input.DatabaseContext.Update(findedUser);
        input.DatabaseContext.SaveChanges();

        // Invalidate user cache
        string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, findedUser.ID);
        IOCache.InvalidateCache(cacheKey);
    }
}
