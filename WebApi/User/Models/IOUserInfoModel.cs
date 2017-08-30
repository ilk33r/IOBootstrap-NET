using System;

namespace IOBootstrap.NET.WebApi.User.Models
{
    public class IOUserInfoModel
    {

		#region Properties

		public int ID { get; set; }
		public string UserName { get; set; }
		public int UserRole { get; set; }
		public string UserToken { get; set; }
		public DateTimeOffset TokenDate { get; set; }

		#endregion

	}
}
