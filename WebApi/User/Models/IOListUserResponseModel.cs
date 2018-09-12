using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.User.Models
{
    public class IOListUserResponseModel : IOResponseModel
    {
		#region Properties

        public IList<IOUserInfoModel> Users { get; }

		#endregion

		#region Initialization Methods

        public IOListUserResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

		public IOListUserResponseModel(IOResponseStatusModel status, IList<IOUserInfoModel> users): base(status) {
			// Setup properties
            this.Users = users;
		}

		#endregion

	}
}
