using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common;

public class IOGenerateBOPageFilesRequestModel : IORequestModel
{
    [Required]
    public string EntityName { get; set; }

    [Required]
    public string EntityDisplayName { get; set; }

    [Required]
    public string EntityItemName { get; set; }

    [Required]
    public IList<IOBOPageEntityModel> Properties { get; set; }

    [Required]
    public string ListEntityName { get; set; }

    // [Required]
    public string ListEntityAPIPath { get; set; }

    // [Required]
    public string ListEntityAction { get; set; }

    [Required]
    public string ListEntityDisplayName { get; set; }

}
