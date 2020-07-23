using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOMessageAddResponseModel : IOResponseModel
    {
        public IOMessageAddResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOMessageAddResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
