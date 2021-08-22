using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Clients;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientListResponseModel : IOResponseModel
    {

        #region Properties

        public IList<IOClientInfoModel> ClientList { get; }

        #endregion

        #region Initialization Methods

        public IOClientListResponseModel(IList<IOClientInfoModel> clientList): base() {
            // Setup properties
            this.ClientList = clientList;
        }

        #endregion

    }
}
