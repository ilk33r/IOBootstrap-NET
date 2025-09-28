using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users;

public class IOUserResetPasswordRequestModel : IORequestModel
{
    [Required]
    [IOBackofficeRequest]
    public string? UserName { get; set; }

    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
}
