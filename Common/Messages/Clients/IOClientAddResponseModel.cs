using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientAddResponseModel : IOResponseModel
    {

		#region Properties

		public IOClientInfoModel Client { get; }

		#endregion

		#region Initialization Methods

		public IOClientAddResponseModel(IOResponseStatusModel status, IOClientInfoModel client): base(status) {
			// Setup properties
            Client = client;
		}

        public IOClientAddResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        #endregion

    }
}
