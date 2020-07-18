using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientListResponseModel : IOResponseModel
    {

        #region Properties

        public IList<IOClientInfoModel> ClientList { get; }

        #endregion

        #region Initialization Methods

        public IOClientListResponseModel(IOResponseStatusModel status, IList<IOClientInfoModel> clientList): base(status) {
            // Setup properties
            this.ClientList = clientList;
        }

        #endregion

    }
}
