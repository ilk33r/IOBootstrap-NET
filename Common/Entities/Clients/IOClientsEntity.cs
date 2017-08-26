using Realms;
using System;

namespace IOBootstrap.NET.Common.Entities.Clients
{
    public class IOClientsEntity : RealmObject
    {

        #region Properties

        [PrimaryKey]
        public int id { get; set; }

        [Indexed]
        public string clientId { get; set; }
        public string clientSecret { get; set; }

        #endregion

    }
}
