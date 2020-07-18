using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
	public class IOUpdateUserRequestModel : IORequestModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }
        public string UserPassword { get; set; }
    }
}
