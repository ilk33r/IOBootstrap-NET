using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Database;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

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

        #region Helper Methods

        public static void AddClient(IIODatabase database) {
            
        }

        public static List<IOClientBackOfficeInfoModel> GetClients(IIODatabase database)
        {
            // Create list for clients
            List<IOClientBackOfficeInfoModel> clientInfos = new List<IOClientBackOfficeInfoModel>();

            // Obtain realm 
            Realm realm = database.GetRealmForMainThread();

            // Check real is not null
            if (realm != null)
            {
                // Obtain clients from realm
                var clients = realm.All<IOClientsEntity>();

                // Check clients is not null
                if (clients != null)
                {
                    // Loop throught clients
                    for (int i = 0; i < clients.Count(); i++)
                    {
                        // Obtain client entity
                        IOClientsEntity client = clients.ElementAt(i);

                        // Create back office info model
                        IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.id, client.clientId, client.clientSecret);

                        // Add model to client info list
                        clientInfos.Add(model);
                    }
                }
            }

            return clientInfos;
        }

        #endregion

    }
}
