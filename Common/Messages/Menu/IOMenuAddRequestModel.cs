using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Menu;

public class IOMenuAddRequestModel : IORequestModel
{

    [IOBackofficeRequest]
    public string? Action { get; set; }

    [IOBackofficeRequest]
    public string? CssClass { get; set; }

    [Required]
    [IOBackofficeRequest]
    public string? Name { get; set; }

    [Required]
    public int? MenuOrder { get; set; }

    [Required]
    public int? RequiredRole { get; set; }

    public int? ParentEntityID { get; set; }
}
