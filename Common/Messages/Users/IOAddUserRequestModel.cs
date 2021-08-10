using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOAddUserRequestModel : IORequestModel
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }
        
        public int UserRole { get; set; }

    }
}
