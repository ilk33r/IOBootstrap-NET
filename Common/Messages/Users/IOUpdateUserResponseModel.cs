using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOUpdateUserResponseModel : IOResponseModel
    {

        public IOUpdateUserResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOUpdateUserResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }
    }
}
