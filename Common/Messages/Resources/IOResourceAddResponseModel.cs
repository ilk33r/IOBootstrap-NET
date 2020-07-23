using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceAddResponseModel : IOResponseModel
    {
        public IOResourceAddResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOResourceAddResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
