using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOClientListResponseModel : IOResponseModel
    {

        #region Properties

        public IList<IOClientBackOfficeInfoModel> ClientList { get; }

        #endregion

        #region Initialization Methods

        public IOClientListResponseModel(IOResponseStatusModel status, IList<IOClientBackOfficeInfoModel> clientList): base(status) {
            // Setup properties
            this.ClientList = clientList;
        }

        #endregion

    }
}
