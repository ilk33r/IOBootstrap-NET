using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Base
{
    public class IOResponseModel : IOModel
    {

        #region Properties

        public IOResponseStatusModel Status { get; set; }

        #endregion

        #region Initialization Methods

        public IOResponseModel(IOResponseStatusModel status) : base()
        {
            // Setup properties
            Status = status;
        }

        public IOResponseModel(int responseStatusMessage) : base()
        {
            Status = new IOResponseStatusModel(responseStatusMessage);
        }

        public IOResponseModel() : base()
        {
            Status = new IOResponseStatusModel(IOResponseStatusMessages.OK);
        }

        #endregion
    }
}
