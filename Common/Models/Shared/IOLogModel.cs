using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Shared;

public class IOLogModel : IOModel
{
    public int? ID { get; set; }

    public DateTimeOffset RequestDate { get; set; }

    public string? IPV4 { get; set; }

    public int? Port { get; set; }

    public int? ResponseCode { get; set; }

    public string? RequestPath { get; set; }

    public string? RequestHeaders { get; set; }

    public string? ResponseHeaders { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }
}
