using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Users
{
    public class IOAddUserResponseModel : IOResponseModel
    {

        #region Properties

        public int UserId { get; set; }
        public string UserName { get; set; }

		#endregion

		#region Initialization Methods

        public IOAddUserResponseModel(int userId, string userName): base() {
			// Setup properties
            UserId = userId;
            UserName = userName;
		}

        #endregion

    }
}
