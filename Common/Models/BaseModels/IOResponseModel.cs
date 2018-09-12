using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.Common.Models.BaseModels
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
            this.Status = status;
        }

        #endregion
    }
}
