using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common;

public class IOGenerateBOPageRequestModel : IORequestModel
{

    [Required]
    public string EntityName { get; set; }

    public IOGenerateBOPageRequestModel() : base()
    {
    }
}
