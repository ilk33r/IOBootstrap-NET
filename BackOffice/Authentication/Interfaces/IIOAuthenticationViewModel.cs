using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Authentication.Interfaces;

public interface IIOAuthenticationViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    #region View Model Methods

    public Task<IOAuthenticationResponseModel> Authenticate(
        string userName,
        string password,
        string? captchaID,
        string? encryptedCaptha
    );

    public IOCheckTokenResponseModel CheckToken(string? token);

    public void Logout(string userName);

    #endregion
}
