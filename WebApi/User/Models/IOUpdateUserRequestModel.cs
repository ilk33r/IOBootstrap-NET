using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.User.Models
{
	public class IOUpdateUserRequestModel : IORequestModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }
        public string UserPassword { get; set; }
    }
}
