using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOCheckTokenResponseModel : IOResponseModel
    {

        public DateTimeOffset TokenLifeTime { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }

        #region Initialization Methods

        public IOCheckTokenResponseModel(DateTimeOffset lifeTime, string userName, int userRole) : base()
        {
            TokenLifeTime = lifeTime;
            UserName = userName;
            UserRole = userRole;
        }

        #endregion
    }
}
