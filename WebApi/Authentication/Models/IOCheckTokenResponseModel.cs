using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Models
{
    public class IOCheckTokenResponseModel : IOResponseModel
    {

        public DateTimeOffset TokenLifeTime { get; set; }

        #region Initialization Methods

        public IOCheckTokenResponseModel(IOResponseStatusModel status, DateTimeOffset lifeTime) : base(status)
        {
            this.TokenLifeTime = lifeTime;
        }

        #endregion
    }
}
