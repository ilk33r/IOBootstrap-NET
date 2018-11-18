using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Core.Firebase.Models
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
