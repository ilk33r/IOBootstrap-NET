using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Models
{
    public class IOCheckTokenResponseModel : IOResponseModel
    {

        public DateTimeOffset TokenLifeTime { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }

        #region Initialization Methods

        public IOCheckTokenResponseModel(IOResponseStatusModel status, DateTimeOffset lifeTime, string userName, int userRole) : base(status)
        {
            this.TokenLifeTime = lifeTime;
            this.UserName = userName;
            this.UserRole = userRole;
        }

        #endregion
    }
}
