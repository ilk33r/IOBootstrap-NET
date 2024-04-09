using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Authentication.Interfaces;

public interface IIOAuthenticationViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    #region View Model Methods

    public IOAuthenticationResponseModel Authenticate(string userName, string password);

    public IOCheckTokenResponseModel CheckToken(string token);

    #endregion
}
