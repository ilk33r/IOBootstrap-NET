using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOSaveImageResponseModel : IOResponseModel
    {
        public IOImageVariationsModel File { get; set; }

        public IOSaveImageResponseModel(IOImageVariationsModel file) : base()
        {
            File = file;
        }
    }
}
