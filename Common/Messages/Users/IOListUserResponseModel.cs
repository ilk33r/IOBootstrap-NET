using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Common.Messages.Users
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
            Users = users;
		}

		#endregion

	}
}
