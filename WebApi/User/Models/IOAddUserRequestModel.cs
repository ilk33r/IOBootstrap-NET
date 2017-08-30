using IOBootstrap.NET.Common.Models.BaseModels;
using System;

namespace IOBootstrap.NET.WebApi.User.Models
{
    public class IOAddUserRequestModel : IORequestModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserRole { get; set; }

    }
}
