using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOCheckTokenResponseModel : IOResponseModel
    {

        public DateTimeOffset TokenLifeTime { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }

        #region Initialization Methods

        public IOCheckTokenResponseModel(IOResponseStatusModel status, DateTimeOffset lifeTime, string userName, int userRole) : base(status)
        {
            TokenLifeTime = lifeTime;
            UserName = userName;
            UserRole = userRole;
        }

        #endregion
    }
}
