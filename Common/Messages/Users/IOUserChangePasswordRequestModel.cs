using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users;

public class IOUserChangePasswordRequestModel : IORequestModel
{
    [Required]
    [DataType(DataType.Password)]
    public string? OldPassword { get; set; }

    [Required]
    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

}
