using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Authentication.Interfaces;

public interface IIOAuthenticationViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    #region View Model Methods

    public Tuple<string, DateTimeOffset, string, int> AuthenticateUser(string userName, string password);

    public Tuple<DateTimeOffset, string, int> CheckToken(string token);

    #endregion
}
