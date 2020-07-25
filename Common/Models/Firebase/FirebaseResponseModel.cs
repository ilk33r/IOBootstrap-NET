using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Firebase
{
	public class FirebaseResponseModel : IOModel
    {
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }

		public FirebaseResponseModel() : base()
        {
        }
    }
}
