using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Files;

public class IODeleteFilesRequestModel : IORequestModel
{
    public int? FileId { get; set; }

    public string? Description { get; set; }
}
