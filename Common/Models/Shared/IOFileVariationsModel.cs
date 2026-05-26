using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Shared;

public class IOFileVariationsModel : IOModel
{
    public int? ID { get; set; }

    [Required]
    [StringLength(128)]
    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public string? PublicId { get; set; }

    public string? Description { get; set; }

    public string? AdditionalData { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}
