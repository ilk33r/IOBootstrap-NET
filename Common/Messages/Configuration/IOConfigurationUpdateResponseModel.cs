using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
    public class IOConfigurationUpdateResponseModel : IOConfigurationAddResponseModel
    {
        public IOConfigurationUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOConfigurationUpdateResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
