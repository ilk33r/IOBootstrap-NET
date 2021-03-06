﻿using System;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Base
{
    public class IOResponseModel : IOModel
    {

        #region Properties

        public IOResponseStatusModel Status { get; }

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

        #endregion
    }
}
