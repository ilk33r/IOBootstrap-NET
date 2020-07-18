using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOAddUserRequestModel : IORequestModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserRole { get; set; }

    }
}
