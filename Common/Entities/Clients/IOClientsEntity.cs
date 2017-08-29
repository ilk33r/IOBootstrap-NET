using IOBootstrap.NET.Common.Entities.AutoIncrements;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
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
        public int ID { get; set; }

        [Indexed]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        #endregion

        #region Helper Methods

        public static IOClientsEntity CreateClient(IIODatabase database) {
            // Create IOClients entity
            IOClientsEntity clientsEntity = new IOClientsEntity()
            {
                ID = IOAutoIncrementsEntity.IdForClass(database, typeof(IOClientsEntity)),
                ClientId = IOCommonHelpers.GenerateRandomAlphaNumericString(16),
                ClientSecret = IOCommonHelpers.GenerateRandomAlphaNumericString(48)
            };

            // Return clients entity
            return clientsEntity;
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
                        IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.ID, client.ClientId, client.ClientSecret);

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
