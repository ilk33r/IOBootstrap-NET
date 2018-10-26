using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOListMessagesResponseModel : IOResponseModel
    {

        public IList<IOMessageModel> Messages { get; set; }

        public IOListMessagesResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

        public IOListMessagesResponseModel(IOResponseStatusModel status, IList<IOMessageModel> messages) : base(status)
        {
            this.Messages = messages;
        }
    }
}
