using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOAuthenticationRequestModel : IORequestModel
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }

    }
}
