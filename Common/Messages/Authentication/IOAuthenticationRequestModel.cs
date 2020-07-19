using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOAuthenticationRequestModel : IORequestModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
