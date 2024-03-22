using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IODeleteImagesRequestModel : IORequestModel
    {
        [Required]
        public int ImageId { get; set; }
    }
}
