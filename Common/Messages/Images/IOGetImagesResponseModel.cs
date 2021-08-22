using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOGetImagesResponseModel : IOResponseModel
    {
        public int Count { get; set; }
        public IList<IOImageVariationsModel> Images { get; set; }

        public IOGetImagesResponseModel(int count, IList<IOImageVariationsModel> images) : base()
        {
            Count = count;
            Images = images;
        }
    }
}
