using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.DataAccess.Entities;

public class IOLogsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    public DateTimeOffset RequestDate { get; set; }

    [StringLength(15)]
    public string? IPV4 { get; set; }

    public int? Port { get; set; }

    public int? ResponseCode { get; set; }

    [StringLength(64)]
    public string? RequestPath { get; set; }

    [StringLength(512)]
    public string? RequestHeaders { get; set; }

    [StringLength(512)]
    public string? ResponseHeaders { get; set; }

    [StringLength(2048)]
    public string? RequestBody { get; set; }

    [StringLength(2048)]
    public string? ResponseBody { get; set; }
}
