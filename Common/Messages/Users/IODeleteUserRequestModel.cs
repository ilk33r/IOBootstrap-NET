using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IODeleteUserRequestModel : IORequestModel
    {

        [Required]
        public int UserId { get; set; }

    }
}
