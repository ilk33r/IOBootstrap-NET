using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication
{
    public class IOCheckTokenRequestModel : IORequestModel
    {
        [Required]
        public string Token { get; set; }
    }
}
