using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOGetImagesRequestModel : IORequestModel
    {
        [Required]
        public int Count { get; set; }

        [Required]
        public int Start { get; set; }
    }
}
