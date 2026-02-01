using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration;

public class IORemoveLogsRequestModel : IORequestModel
{
    [Required]
    public DateTimeOffset? StartDate { get; set; }
}
