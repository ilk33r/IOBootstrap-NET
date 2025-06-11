using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users;

public class IOAddUserRequestModel : IORequestModel
{

    [Required]
    [IOBackofficeRequest]
    public string? UserName { get; set; }

    [Required]
    [MinLength(4)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public int UserRole { get; set; }

    [Required]
    public bool? IsActive { get; set; }

    [Required]
    public DateTimeOffset? ActivationEndDate { get; set; }
}
