using System;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages
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
