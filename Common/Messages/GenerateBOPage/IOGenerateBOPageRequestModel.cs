using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.GenerateBOPage;

public class IOGenerateBOPageRequestModel : IORequestModel
{

    [Required]
    public string? EntityName { get; set; }
}
