using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOClientAddResponseModel : IOResponseModel
    {

		#region Properties

		public IOClientBackOfficeInfoModel Client { get; }

		#endregion

		#region Initialization Methods

		public IOClientAddResponseModel(IOResponseStatusModel status, IOClientBackOfficeInfoModel client): base(status) {
			// Setup properties
            this.Client = client;
		}

		#endregion

	}
}
