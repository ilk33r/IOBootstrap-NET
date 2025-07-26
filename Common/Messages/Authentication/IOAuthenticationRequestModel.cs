using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication;

public class IOAuthenticationRequestModel : IORequestModel
{

    [Required]
    [IOBackofficeRequest]
    public string? UserName { get; set; }

    [Required]
    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? CaptchaID { get; set; }

    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? EncryptedCaptcha { get; set; }

}
