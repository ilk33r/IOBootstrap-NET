using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.WebApi.User.Models
{
    public class IOAddUserResponseModel : IOResponseModel
    {

        #region Properties

        public int UserId { get; set; }
        public string UserName { get; set; }

		#endregion

		#region Initialization Methods

        public IOAddUserResponseModel(IOResponseStatusModel status, int userId, string userName): base(status) {
			// Setup properties
            this.UserId = userId;
            this.UserName = userName;
		}

		#endregion

	}
}
