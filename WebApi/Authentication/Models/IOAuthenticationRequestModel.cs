using IOBootstrap.NET.Common.Models.BaseModels;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Models
{
    public class IOAuthenticationRequestModel : IORequestModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
