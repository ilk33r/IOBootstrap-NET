using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Menu
{
	public class IOMenuUpdateResponseModel : IOMenuAddResponseModel
    {
        public IOMenuUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOMenuUpdateResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
