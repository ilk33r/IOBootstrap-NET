using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Clients;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientAddResponseModel : IOResponseModel
    {

		#region Properties

		public IOClientInfoModel Client { get; }

		#endregion

		#region Initialization Methods

		public IOClientAddResponseModel(IOClientInfoModel client): base() {
			// Setup properties
            Client = client;
		}

        #endregion

    }
}
