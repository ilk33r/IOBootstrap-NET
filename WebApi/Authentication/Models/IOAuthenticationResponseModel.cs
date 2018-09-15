using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Models
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
            this.Token = token;
            this.TokenLifeTime = lifeTime;
            this.UserName = userName;
            this.UserRole = userRole;
        }

        #endregion

    }
}
