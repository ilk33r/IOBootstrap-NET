using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Menu
{
    public class IOMenuAddResponseModel : IOResponseModel
    {
        public IOMenuAddResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOMenuAddResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
