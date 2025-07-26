using System;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Constants;

namespace IOBootstrap.NET.BackOffice.Authentication.ViewModels;

public abstract class IOAuthenticationViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOAuthenticationViewModel<TDBContext>, IIOAuthentication<TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{

    #region Initialization Methods

    public IOAuthenticationViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public virtual async Task<IOAuthenticationResponseModel> Authenticate(
        string userName,
        string password,
        string? captchaID,
        string? encryptedCaptha
    )
    {
        IOAuthenticationResponseModel response = await this.AuthenticateUser(
            userName,
            password,
            captchaID,
            encryptedCaptha
        );
        if (response.UserRole >= (int)UserRoles.BackOfficeUser)
        {
            throw new IOInvalidPermissionException();
        }

        return response;
    }

    public virtual IOCheckTokenResponseModel CheckToken(string? token)
    {
        bool cookieAuthentication = Configuration.GetValue<bool>(IOConfigurationConstants.CookieAuthentication);
        string? appToken;

        if (cookieAuthentication && Request.Cookies.ContainsKey(IOCookieConstants.TokenCookieName))
        {
            // Obtain token
            appToken = Request.Cookies[IOCookieConstants.TokenCookieName]!;
        }
        else if (!cookieAuthentication) {
            appToken = token;
        }
        else
        {
            throw new IOInvalidPermissionException();
        }

        if (appToken == null)
        {
            throw new IOInvalidPermissionException();
        }

        IOCheckTokenResponseModel response = this.CheckUserToken(appToken);
        if (response.UserRole >= (int)UserRoles.BackOfficeUser)
        {
            throw new IOInvalidPermissionException();
        }

        return response;
    }

    public virtual void Logout(string userName)
    {
        this.LogoutUser(userName);
    }

    #endregion

}
