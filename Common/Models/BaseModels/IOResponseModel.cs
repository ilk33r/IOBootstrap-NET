using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.Common.Models.BaseModels
{
    public class IOResponseModel
    {

        #region Properties

        public IOResponseStatusModel Status { get; }

        #endregion

        #region Initialization Methods

        public IOResponseModel(IOResponseStatusModel status)
        {
            // Setup properties
            this.Status = status;
        }

        #endregion
    }
}
