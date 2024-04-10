using System;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;

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

    public virtual IOAuthenticationResponseModel Authenticate(string userName, string password)
    {
        IOAuthenticationResponseModel response = this.AuthenticateUser(userName, password);
        
        if (response.UserRole >= (int)UserRoles.BackOfficeUser)
        {
            throw new IOInvalidPermissionException();
        }

        return response;
    }

    public virtual IOCheckTokenResponseModel CheckToken(string token)
    {
        IOCheckTokenResponseModel response = this.CheckUserToken(token);

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
