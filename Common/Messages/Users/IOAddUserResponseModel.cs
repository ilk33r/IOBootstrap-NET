using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Users
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
            UserId = userId;
            UserName = userName;
		}

        public IOAddUserResponseModel(int responseStatusMessage) : base(responseStatusMessage)
        {
        }

        #endregion

    }
}
