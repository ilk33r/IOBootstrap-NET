using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.DataAccess.Entities;

public class IOFilesEntity
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required]
    [StringLength(128)]
    public string? FileName { get; set; }

    [StringLength(32)]
    public string? FileType { get; set; }

    [Required]
    [StringLength(128)]
    public string? Description { get; set; }

    [StringLength(128)]
    public string? AdditionalData { get; set; }

    [StringLength(255)]
    public string? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}
