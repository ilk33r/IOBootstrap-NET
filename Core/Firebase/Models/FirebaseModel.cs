using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Core.Firebase.Models
{
	public class FirebaseModel : IOModel
    {

        public IList<string> registration_ids { get; set; }
        public FirebaseDataModel data { get; set; }

        public FirebaseModel(IList<string> registrationIds, FirebaseDataModel data) : base()
        {
            this.registration_ids = registrationIds;
            this.data = data;
        }
    }
}
