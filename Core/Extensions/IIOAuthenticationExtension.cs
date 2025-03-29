using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOAuthenticationExtension
{

    public static IOAuthenticationResponseModel AuthenticateUser<TDBContext>(this IIOAuthentication<TDBContext> input, string userName, string password)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Decrypt password
        string decryptedPassword = input.DecryptString(password);
        
        IOUserEntity? findedUser = input.DatabaseContext.Users
                                                    .Where(u => u.UserName!.Equals(userName))
                                                    .FirstOrDefault();

        if (findedUser == null)
        {
            // Return response
            throw new IOInvalidCredentialsException();
        }

        // Check user is active
        input.CheckUserActivationStatus(findedUser);

        // Check user is locked
        input.CheckUserIsLocked(findedUser);

        // Check user password is wrong
        if (!IOPasswordUtilities.VerifyPassword(decryptedPassword, findedUser.Password ?? ""))
        {
            // Update User
            findedUser.WrongPasswordAttemptCount += 1;
            findedUser.LastWrongPasswordAttemptDate = DateTimeOffset.UtcNow;
            input.DatabaseContext.Update(findedUser);
            input.DatabaseContext.SaveChanges();

            // Return response
            throw new IOInvalidCredentialsException();
        }

        // Generate token for user
        string userTokenString = IORandomUtilities.GenerateGUIDString();

        // Create decrypted user token string
        string decryptedUserToken = String.Format("{0},{1}", findedUser.ID, userTokenString);

        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        string userNewToken = Convert.ToBase64String(aesUtilities.Encrypt(decryptedUserToken));

        // Create token date
        DateTime tokenDate = DateTime.UtcNow;

        // Obtain token life from configuration
        int tokenLife = input.Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

        // Update entity properties
        findedUser.UserToken = userTokenString;
        findedUser.TokenDate = tokenDate;

        input.DatabaseContext.Update(findedUser);
        input.DatabaseContext.SaveChanges();

        // Invalidate user cache
        string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, findedUser.ID);
        IOCache.InvalidateCache(cacheKey);

        // Encrypt sensitive data
        string encryptedUserName = input.EncryptString(findedUser.UserName ?? "");
        string encryptedToken = input.EncryptString(userNewToken);

        // Return response
        return new IOAuthenticationResponseModel(encryptedToken, tokenDate.Add(new TimeSpan(tokenLife * 1000)), encryptedUserName, findedUser.UserRole);
    }

    public static IOCheckTokenResponseModel CheckUserToken<TDBContext>(this IIOAuthentication<TDBContext> input, string token)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Parse token data
        Tuple<string, int> tokenData = input.ParseUserToken(token);

        IOUserInfoModel? findedUser = input.DatabaseContext.Users
                                                    .Select(u => new IOUserInfoModel()
                                                    {
                                                        ID = u.ID,
                                                        Password = u.Password,
                                                        UserName = u.UserName,
                                                        UserRole = u.UserRole,
                                                        UserToken = u.UserToken,
                                                        TokenDate = u.TokenDate,
                                                        IsActive = u.IsActive,
                                                        ActivationEndDate = u.ActivationEndDate
                                                    })
                                                    .Where(u => u.ID == tokenData.Item2)
                                                    .FirstOrDefault();

        if (findedUser == null)
        {
            throw new IOInvalidCredentialsException();
        }

        // Check user is active
        if (findedUser.IsActive)
        {
            // Obtain current unix time
            long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long activationEndUnixTime = findedUser.ActivationEndDate.ToUnixTimeSeconds();

            // Check activation is ended
            if (currentUnixTime > activationEndUnixTime)
            {
                // Throw an exception
                throw new IOUserDeactivatedException();
            }
        }
        else
        {
            // Throw an exception
            throw new IOUserDeactivatedException();
        }

        // Obtain token life from configuration
        int tokenLife = input.Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

        // Calculate token end seconds and current seconds
        long currentSeconds = IODateTimeUtilities.UnixTimeFromDate(DateTime.UtcNow);
        long tokenEndSeconds = IODateTimeUtilities.UnixTimeFromDate(findedUser.TokenDate.DateTime) + tokenLife;

        // Compare user token
        if (findedUser.UserToken != null && currentSeconds < tokenEndSeconds && findedUser.UserToken.Equals(tokenData.Item1))
        {
            // Encrypt sensitive data
            string encryptedUserName = input.EncryptString(findedUser.UserName ?? "");

            // Return status
            return new IOCheckTokenResponseModel(findedUser.TokenDate.DateTime, encryptedUserName, findedUser.UserRole);
        }

        // Return status
        throw new IOInvalidCredentialsException();
    }

    private static void CheckUserActivationStatus<TDBContext>(this IIOAuthentication<TDBContext> input, IOUserEntity user)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Check user is active
        if (user.IsActive)
        {
            // Obtain current unix time
            long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long activationEndUnixTime = user.ActivationEndDate.ToUnixTimeSeconds();

            // Check activation is ended
            if (currentUnixTime > activationEndUnixTime)
            {
                // Update User
                user.IsActive = false;
                input.DatabaseContext.Update(user);
                input.DatabaseContext.SaveChanges();

                // Throw an exception
                throw new IOUserDeactivatedException();
            }
        }
        else
        {
            // Throw an exception
            throw new IOUserDeactivatedException();
        }
    }

    private static void CheckUserIsLocked<TDBContext>(this IIOAuthentication<TDBContext> input, IOUserEntity user)
    where TDBContext : IODatabaseContext<TDBContext>
    {
        // Check password attempt count
        if (user.WrongPasswordAttemptCount < 3)
        {
            // Then do nothing
            return;
        }

        // Obtain current unix time
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long lastWrongPasswordAttemptUnixTime = user.LastWrongPasswordAttemptDate.ToUnixTimeSeconds();

        // Obtain token life from configuration
        int tokenLife = input.Configuration.GetValue<int>(IOConfigurationConstants.TokenLife);

        // Check last password attempt unix time
        if (currentUnixTime > lastWrongPasswordAttemptUnixTime + tokenLife)
        {
            // Then update wrong password attempt count
            user.WrongPasswordAttemptCount = 0;
            input.DatabaseContext.Update(user);
            input.DatabaseContext.SaveChanges();

            // Then do nothing
            return;
        }

        // Throw an exception
        throw new IOUserDeactivatedException();
    }
}
