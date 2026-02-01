using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
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
    private static string CaptchaCacheName = "auth-captcha-{0}";

    public static async Task<IOAuthenticationResponseModel> AuthenticateUser<TDBContext>(
        this IIOAuthentication<TDBContext> input,
        string userName,
        string password,
        string? captchaID,
        string? encryptedCaptha
    )
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        // Decrypt password
        string decryptedPassword = await input.DecryptString(password);

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

        // Check captcha status
        string? requiredCaptchaID = input.CheckCaptchaIsRequired(findedUser);

        // Check captcha is required
        if (requiredCaptchaID != null)
        {
            // Then validate captcha
            bool captchaValidationStatus = await input.ValidateCaptcha(findedUser, captchaID, encryptedCaptha);

            // Check captcha is valid
            if (!captchaValidationStatus)
            {
                // Return response
                throw new IOCaptchaRequiredException(requiredCaptchaID ?? "");
            }
        }

        // Check user is locked
        input.CheckUserIsLocked(findedUser);

        // Check user password is wrong
        using (var passwordUtilities = new IOPasswordUtilities())
        {
            await passwordUtilities.VerifyPassword(decryptedPassword, findedUser.Password ?? "", verified =>
            {
                // Check password is not verified
                if (!verified)
                {
                    // Update User
                    findedUser.WrongPasswordAttemptCount += 1;
                    findedUser.LastWrongPasswordAttemptDate = DateTimeOffset.UtcNow;
                    input.DatabaseContext.Update(findedUser);
                    input.DatabaseContext.SaveChanges();

                    // Return response
                    throw new IOInvalidCredentialsException(requiredCaptchaID ?? "");
                }
            });
        }

        // Generate token for user
        string userTokenString = IORandomUtilities.GenerateGUIDString();

        // Create token
        string userNewToken = input.CreateTokenExtras(
            [
                findedUser.ID,
                userTokenString
            ]
        );

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
        string tokenExtras = input.CreateTokenExtras(
            [
                findedUser.ID,
                findedUser.UserName ?? String.Empty, 
                findedUser.UserRole.ToString()
            ]
        );

        // Return response
        return new IOAuthenticationResponseModel(
            userNewToken,
            tokenDate.Add(new TimeSpan(tokenLife * 1000)),
            tokenExtras,
            findedUser.UserRole
        );
    }

    public static string CreateTokenExtras<TDBContext>(
        this IIOAuthentication<TDBContext> input,
        params object?[] args
    )
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        List<string> formatStringList = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            formatStringList.Add(
                String.Format("{{{0}}}", i)
            );
        }

        // Create decrypted user token string
        string formatString = String.Join(";", formatStringList);
        string decryptedUserTokenExtras = String.Format(formatString, args);

        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(input.Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        return Convert.ToBase64String(aesUtilities.Encrypt(decryptedUserTokenExtras));
    }

    public static IOCheckTokenResponseModel CheckUserToken<TDBContext>(this IIOAuthentication<TDBContext> input, string token, string extras)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        // Parse token data
        Tuple<string, int> tokenData = input.ParseUserToken(token);
            
        if (tokenData.Item2 > 0)
        {

            IOUserInfoModel? findedUser;
        
            // Obtain user entity from database
            string cacheKey = String.Format(IOCacheKeys.BackOfficeUserCacheKey, tokenData.Item2);
            IOCacheObject? userCache = IOCache.GetCachedObject(cacheKey);

            if (userCache != null)
            {
                findedUser = (IOUserInfoModel)userCache.Value;
            }
            else
            {
                findedUser = input.DatabaseContext.Users
                                            .Select(u => new IOUserInfoModel()
                                            {
                                                ID = u.ID,
                                                Password = u.Password,
                                                UserName = u.UserName,
                                                UserRole = u.UserRole,
                                                UserToken = u.UserToken,
                                                TokenDate = u.TokenDate,
                                                IsActive = u.IsActive,
                                                ActivationEndDate = u.ActivationEndDate,
                                                PasswordExpireDate = u.PasswordExpireDate
                                            })
                                            .Where(u => u.ID == tokenData.Item2)
                                            .FirstOrDefault();
            }

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
                input.TokenExtras = input.ParseUserTokenExtras(extras);

                if (input.TokenExtras.Length == 0 || int.Parse(input.TokenExtras[0]) != tokenData.Item2)
                {
                    throw new IOInvalidCredentialsException();
                }

                if (userCache == null)
                {
                    userCache = new IOCacheObject(cacheKey, findedUser, 60);
                    IOCache.CacheObject(userCache);
                }

                // Return status
                return new IOCheckTokenResponseModel(findedUser.TokenDate.DateTime, [.. input.TokenExtras]);
            }
        }

        // Return status
        throw new IOInvalidCredentialsException();
    }

    public static void CheckUserActivationStatus<TDBContext>(this IIOAuthentication<TDBContext> input, IOUserEntity user)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
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

    public static string? CheckCaptchaIsRequired<TDBContext>(
        this IIOAuthentication<TDBContext> input,
        IOUserEntity user
    )
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        // Check password attempt count
        if (user.WrongPasswordAttemptCount < 3)
        {
            // Then do nothing
            return null;
        }

        // Captcha cache key
        string captchaID = IORandomUtilities.GenerateGUIDString();
        string cacheKey = String.Format(CaptchaCacheName, captchaID);

        // Cache captcha
        string newCaptcha = IORandomUtilities.GenerateRandomAlphaNumericString(6);
        IOCacheObject captchaCache = new IOCacheObject(cacheKey, newCaptcha, 300);
        IOCache.CacheObject(captchaCache);

        // Then return captcha id
        return captchaID;
    }

    public static async Task<bool> ValidateCaptcha<TDBContext>(
        this IIOAuthentication<TDBContext> input,
        IOUserEntity user,
        string? captchaID,
        string? encryptedCaptha
    )
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        // Check captcha id is exists
        if (captchaID == null)
        {
            return false;
        }
        
        // Captcha cache key
        string cacheKey = String.Format(CaptchaCacheName, captchaID);

        // Obtain cached captcha
        IOCacheObject? captchaCache = IOCache.GetCachedObject(cacheKey);
        if (captchaCache == null)
        {
            return false;
        }

        // Obtain captcha string value
        string captchaCacheValue = (string)captchaCache.Value;

        // Check encrypted captcha
        if (encryptedCaptha == null)
        {
            return false;
        }
        
        // Decrypt captcha
        string decryptCaptcha = await input.DecryptString(encryptedCaptha);

        // Then return captcha validation status
        return decryptCaptcha.Equals(captchaCacheValue);
    }

    private static void CheckUserIsLocked<TDBContext>(this IIOAuthentication<TDBContext> input, IOUserEntity user)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    {
        // Check password attempt count
        int currentWrongPasswordAttemptCount = user.WrongPasswordAttemptCount;

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

        if (currentWrongPasswordAttemptCount < 7)
        {
            // Then do nothing
            return;
        }

        // Throw an exception
        throw new IOUserDeactivatedException();
    }
}
