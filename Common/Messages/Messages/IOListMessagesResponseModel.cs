using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOListMessagesResponseModel : IOResponseModel
    {

        public IList<IOMessageModel> Messages { get; set; }

        public IOListMessagesResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOListMessagesResponseModel(int responseStatusMessage, IList<IOMessageModel> messages) : base(responseStatusMessage)
        {
            Messages = messages;
        }
    }
}
