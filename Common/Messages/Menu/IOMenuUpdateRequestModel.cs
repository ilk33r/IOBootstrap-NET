using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Menu;

public class IOMenuUpdateRequestModel : IOMenuAddRequestModel
{
    [Required]
    public int? ID { get; set; }

    public IOMenuUpdateRequestModel() : base()
    {
    }
}
