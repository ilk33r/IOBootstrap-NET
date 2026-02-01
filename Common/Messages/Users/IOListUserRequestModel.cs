using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users;

public class IOListUserRequestModel : IORequestModel
{
    [Required]
    public int? Count { get; set; }

    [Required]
    public int? Start { get; set; }
}
