using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOGetImagesRequestModel : IORequestModel
    {
        public int Count { get; set; }
        public int Start { get; set; }
    }
}
