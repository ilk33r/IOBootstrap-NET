using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Images
{
    public class IOSaveImageResponseModel : IOResponseModel
    {
        public IList<IOImageVariationsModel> Files { get; set; }

        public IOSaveImageResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        public IOSaveImageResponseModel(int responseStatusMessage, IList<IOImageVariationsModel> files) : base(responseStatusMessage)
        {
            Files = files;
        }
    }
}
