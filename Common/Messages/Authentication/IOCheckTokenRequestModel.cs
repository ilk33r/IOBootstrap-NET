using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOCheckTokenRequestModel : IORequestModel
    {
        public string Token { get; set; }
    }
}
