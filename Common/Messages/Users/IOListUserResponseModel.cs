using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOListUserResponseModel : IOResponseModel
    {
		#region Properties

        public IList<IOUserInfoModel> Users { get; }

		#endregion

		#region Initialization Methods

		public IOListUserResponseModel(IList<IOUserInfoModel> users): base() {
			// Setup properties
            Users = users;
		}

		#endregion

	}
}
