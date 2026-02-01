using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication;

public class IOCheckTokenResponseModel : IOResponseModel
{

    public DateTimeOffset TokenLifeTime { get; set; }
    public IList<string> Extras { get; set; }

    #region Initialization Methods

    public IOCheckTokenResponseModel(DateTimeOffset lifeTime, IList<string> extras) : base()
    {
        TokenLifeTime = lifeTime;
        Extras = extras;
    }

    #endregion
}
