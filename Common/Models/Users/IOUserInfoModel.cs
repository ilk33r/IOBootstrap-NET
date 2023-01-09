using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Users
{
    public class IOUserInfoModel : IOModel
    {

		#region Properties

		public int ID { get; set; }
		public string Password { get; set; }
		public string UserName { get; set; }
		public int UserRole { get; set; }
		public string UserToken { get; set; }
		public DateTimeOffset TokenDate { get; set; }

		#endregion

	}
}
