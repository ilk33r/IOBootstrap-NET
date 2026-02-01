using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication;

public class IOAuthenticationResponseModel : IOResponseModel
{
    public string? Token { get; set; }
    public DateTimeOffset TokenLifeTime { get; set; }
    public string? Extras { get; set; }
    public int UserRole { get; set; }

    #region Initialization Methods

    public IOAuthenticationResponseModel(string token, DateTimeOffset lifeTime, string? extras, int userRole) : base()
    {
        Token = token;
        TokenLifeTime = lifeTime;
        Extras = extras;
        UserRole = userRole;
    }

    #endregion

}
