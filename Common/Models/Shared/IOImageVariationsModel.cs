using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Shared
{
    public class IOImageVariationsModel : IOModel
    {
        public int? ID { get; set; }
        
        [Required]
        [StringLength(128)]
        public string FileName { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? Scale { get; set; }

        public bool KeepRatio { get; set; }
    }
}
