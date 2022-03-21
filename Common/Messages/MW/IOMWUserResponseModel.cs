using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWUserResponseModel : IOResponseModel
    {
        #region Properties

		public int ID { get; set; }
		public string Password { get; set; }
		public string UserName { get; set; }
		public int UserRole { get; set; }
		public string UserToken { get; set; }
		public DateTimeOffset TokenDate { get; set; }

		#endregion

        #region Initialization Methods

		public IOMWUserResponseModel() : base() 
		{
		}

		public IOMWUserResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

		#endregion
    }
}
