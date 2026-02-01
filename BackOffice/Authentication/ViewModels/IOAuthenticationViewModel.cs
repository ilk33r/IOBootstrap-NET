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
where TDBContext : IOBaseDatabaseContext<TDBContext>
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

    public virtual async Task<IOCheckTokenResponseModel> CheckToken(string? token, string? tokenExtras)
    {
        bool cookieAuthentication = Configuration.GetValue<bool>(IOConfigurationConstants.CookieAuthentication);
        string? appToken;
        string? appTokenExtras;

        if (cookieAuthentication && Request.Cookies.ContainsKey(IOCookieConstants.TokenCookieName))
        {
            // Obtain token
            appToken = Request.Cookies[IOCookieConstants.TokenCookieName]!;
            appTokenExtras = Request.Cookies[IOCookieConstants.TokenExtrasCookieName]!;
        }
        else if (!cookieAuthentication) {
            appToken = token;
            appTokenExtras = tokenExtras;
        }
        else
        {
            throw new IOInvalidPermissionException();
        }

        if (appToken == null)
        {
            throw new IOInvalidPermissionException();
        }

        IOCheckTokenResponseModel response = this.CheckUserToken(appToken, appTokenExtras ?? String.Empty);
        int extrasCount = response.Extras?.Count ?? 0;
        int userRole;
        if (extrasCount > 2)
        {
            int? extrasRole = int.Parse(response.Extras?[2] ?? "0");
            userRole = extrasRole ?? (int)UserRoles.AnonmyMouse;
        }
        else
        {
            userRole = (int)UserRoles.AnonmyMouse;
        }

        if (userRole >= (int)UserRoles.BackOfficeUser)
        {
            throw new IOInvalidPermissionException();
        }

        List<string> encryptedExtras = new List<string>();
        foreach (var item in response.Extras ?? [])
        {
            string encryptedData = await EncryptString(item ?? String.Empty);
            encryptedExtras.Add(encryptedData);
        }

        response.Extras = encryptedExtras;
        return response;
    }

    public virtual async Task Logout(string userName)
    {
        await this.LogoutUser(userName);
    }

    #endregion

}
