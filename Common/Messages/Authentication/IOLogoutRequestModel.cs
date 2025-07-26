using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Authentication;

public class IOLogoutRequestModel : IORequestModel
{

    [Required]
    public string? UserName { get; set; }
}
