using IOBootstrap.NET.Common.Models.BaseModels;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Models
{
    public class IOCheckTokenRequestModel : IORequestModel
    {
        public string Token { get; set; }
    }
}
