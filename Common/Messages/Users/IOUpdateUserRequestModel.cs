using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
	public class IOUpdateUserRequestModel : IORequestModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public int UserRole { get; set; }

        public string UserPassword { get; set; }
    }
}
