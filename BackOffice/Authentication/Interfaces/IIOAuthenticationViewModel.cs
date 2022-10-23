using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.BackOffice.Authentication.Interfaces;

public interface IIOAuthenticationViewModel : IIOBackOfficeViewModel
{
    #region View Model Methods

    public Tuple<string, DateTimeOffset, string, int> AuthenticateUser(string userName, string password);

    public Tuple<DateTimeOffset, string, int> CheckToken(string token);

    #endregion
}
