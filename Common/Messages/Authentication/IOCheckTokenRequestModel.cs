using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication;

public class IOCheckTokenRequestModel : IORequestModel
{
    [IOBackofficeRequest]
    public string? Token { get; set; }

    [IOBackofficeRequest]
    public string? Extras { get; set; }
}
