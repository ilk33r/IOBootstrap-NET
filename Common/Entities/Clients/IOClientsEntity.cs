using Realms;

namespace IOBootstrap.NET.Common.Entities.Clients
{
    public class IOClientsEntity : RealmObject
    {

        #region Properties

        [PrimaryKey]
        public int ID { get; set; }

        [Indexed]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
        public string ClientDescription { get; set; }

        #endregion

    }
}
