using System;
using Realms;

namespace IOBootstrap.NET.Common.Entities.Users
{
    public class IOUserEntity : RealmObject
    {

		#region Properties

		[PrimaryKey]
		public int ID { get; set; }

		[Indexed]
		public string UserName { get; set; }

		public string Password { get; set; }
        public int UserRole { get; set; }
        public string UserToken { get; set; }
        public DateTimeOffset TokenDate { get; set; }

		#endregion

	}
}
