using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOAuthenticationResponseModel : IOResponseModel
    {
        public string Token { get; set; }
        public DateTimeOffset TokenLifeTime { get; set; }
        public string UserName { get; set; } 
        public int UserRole { get; set; }

        #region Initialization Methods


        public IOAuthenticationResponseModel(IOResponseStatusModel status, string token, DateTimeOffset lifeTime, string userName, int userRole) : base(status)
        {
            Token = token;
            TokenLifeTime = lifeTime;
            UserName = userName;
            UserRole = userRole;
        }

        public IOAuthenticationResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOAuthenticationResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        #endregion

    }
}
