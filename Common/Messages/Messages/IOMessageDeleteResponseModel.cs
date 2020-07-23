using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageDeleteResponseModel : IOResponseModel
    {
        public IOMessageDeleteResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOMessageDeleteResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
