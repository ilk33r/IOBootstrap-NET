using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration;

public class IOConfigurationAddRequestModel : IORequestModel
{

    [Required]
    [IOBackofficeRequest]
    public string? ConfigKey { get; set; }
    public string? StrValue { get; set; }
    public int? IntValue { get; set; }
}
