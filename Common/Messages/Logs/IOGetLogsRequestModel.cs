using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Logs;

public class IOGetLogsRequestModel : IORequestModel
{
    [Required]
    public int? Count { get; set; }

    [Required]
    public int? Start { get; set; }
}
