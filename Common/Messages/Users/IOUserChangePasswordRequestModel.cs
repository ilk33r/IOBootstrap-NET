using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOUserChangePasswordRequestModel : IORequestModel
    {

        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
}
