using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Messages;

namespace IOBootstrap.NET.Common.Messages.Messages
{
    public class IOListMessagesResponseModel : IOResponseModel
    {

        public IList<IOMessageModel> Messages { get; set; }

        public IOListMessagesResponseModel(IList<IOMessageModel> messages) : base()
        {
            Messages = messages;
        }
    }
}
