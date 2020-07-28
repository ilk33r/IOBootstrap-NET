using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOSaveImageRequestModel : IORequestModel
    {
        [Required]
        public string FileData { get; set; }

        [Required]
        public IList<IOImageVariationsModel> Sizes { get; set; }
    }
}
