using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOUserChangePasswordRequestModel : IORequestModel
    {

        [Required]
        public string UserName { get; set; }

        public string OldPassword { get; set; }

        [Required]
        [MinLength(4)]
        public string NewPassword { get; set; }

    }
}
