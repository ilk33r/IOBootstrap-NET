using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceUpdateResponseModel : IOResponseModel
    {
        public IOResourceUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOResourceUpdateResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}