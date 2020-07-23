using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceDeleteResponseModel : IOResponseModel
    {
        public IOResourceDeleteResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOResourceDeleteResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}