using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
    public class IOConfigurationAddResponseModel : IOResponseModel
    {
        public IOConfigurationAddResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOConfigurationAddResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
