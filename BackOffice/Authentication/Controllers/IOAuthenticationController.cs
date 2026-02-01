using System;
using System.Threading.Tasks;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers;

[IOBackoffice]
public abstract class IOAuthenticationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
where TViewModel : IIOAuthenticationViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    protected IOAuthenticationController(IConfiguration configuration,
                                         IWebHostEnvironment environment,
                                         ILogger<IOLoggerType> logger,
                                         TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Authentication Api

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 10)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IONonceRequired]
    [IOIgnorePasswordExpire]
    [HttpPost("[action]")]
    public virtual async Task<IOAuthenticationResponseModel> Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
    {
        // Check if authentication result is true
        IOAuthenticationResponseModel response = await ViewModel.Authenticate(
            requestModel.UserName ?? "",
            requestModel.Password ?? "",
            requestModel.CaptchaID,
            requestModel.EncryptedCaptcha
        );

        #if DEBUG
        IOCacheObject tokenCacheObject = new IOCacheObject(IOCacheKeys.SwaggerToken, response.Token ?? string.Empty, 0);
        IOCacheObject tokenExtraCacheObject = new IOCacheObject(IOCacheKeys.SwaggerTokenExtra, response.Extras ?? string.Empty, 0);

        IOCache.CacheObject(tokenCacheObject);
        IOCache.CacheObject(tokenExtraCacheObject);
        #endif

        bool cookieAuthentication = Configuration.GetValue<bool>(IOConfigurationConstants.CookieAuthentication);
        if (cookieAuthentication)
        {
            HttpContext.Response.Cookies.Append(IOCookieConstants.TokenCookieName, response.Token ?? string.Empty, new CookieOptions()
            {
                Secure = true,
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddHours(24),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

            response.Token = null;

            HttpContext.Response.Cookies.Append(IOCookieConstants.TokenExtrasCookieName, response.Extras ?? string.Empty, new CookieOptions()
            {
                Secure = true,
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddHours(24),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

            response.Extras = null;
        }

        return response;
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 20)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IONonceRequired]
    [IOIgnorePasswordExpire]
    [HttpPost("[action]")]
    public virtual async Task<IOCheckTokenResponseModel> CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
    {
        // Check if authentication result is true
        return await ViewModel.CheckToken(requestModel.Token, requestModel.Extras);
    }

    #endregion
}
