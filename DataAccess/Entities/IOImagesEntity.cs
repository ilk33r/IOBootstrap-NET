using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.DataAccess.Entities
{
    public class IOImagesEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Required]
        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(32)]
        public string FileType { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? Scale { get; set; }
    }
}
