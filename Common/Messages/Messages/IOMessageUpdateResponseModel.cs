using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Messages
{
	public class IOMessageUpdateResponseModel : IOResponseModel
    {
        public IOMessageUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOMessageUpdateResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
