using System;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOClientInfoModel CreateClient(string clientDescription, long maxRequestCount)
        {
            // Create a client entity
            IOClientsEntity clientEntity = new IOClientsEntity()
            {
                ClientId = IORandomUtilities.GenerateGUIDString(),
                ClientSecret = IORandomUtilities.GenerateGUIDString(),
                ClientDescription = clientDescription,
                IsEnabled = 1,
                RequestCount = 0,
                MaxRequestCount = maxRequestCount
            };

            // Write client to database
            DatabaseContext.Clients.Add(clientEntity);
            DatabaseContext.SaveChanges();

            // Create and return client info
            return new IOClientInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret, clientEntity.ClientDescription, 1, 0, maxRequestCount);
        }

        public bool DeleteClient(IOClientDeleteRequestModel requestModel)
        {            
            IOClientsEntity clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

            // Check client entity is not null
            if (clientEntity == null)
            {
                return false;
            }

            // Delete all entity
            DatabaseContext.Remove(clientEntity);
            DatabaseContext.SaveChanges();
            return true;
        }

        public virtual IList<IOClientInfoModel> GetClients()
        {
            // Create list for clients
            List<IOClientInfoModel> clientInfos = new List<IOClientInfoModel>();

            // Obtain clients from realm
            var clients = DatabaseContext.Clients;

            // Check clients is not null
            if (clients != null)
            {
                List<IOClientsEntity> clientsEntity = clients.ToList();
                clientInfos = clientsEntity.ConvertAll(client =>
                {
                    // Create back office info model
                    return new IOClientInfoModel(client.ID,
                                                client.ClientId,
                                                client.ClientSecret,
                                                client.ClientDescription,
                                                client.IsEnabled,
                                                client.RequestCount,
                                                client.MaxRequestCount);
                });
            }

            // Return clients
            return clientInfos;
        }

        public bool UpdateClient(IOClientUpdateRequestModel requestModel)
        {            
            // Obtain client entity
            IOClientsEntity clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

            // Check client finded
            if (clientEntity != null)
            {
                // Update client properties
                clientEntity.ClientDescription = requestModel.ClientDescription;
                clientEntity.IsEnabled = requestModel.IsEnabled;
                clientEntity.RequestCount = requestModel.RequestCount;
                clientEntity.MaxRequestCount = requestModel.MaxRequestCount;

                // Update client
                DatabaseContext.Update(clientEntity);
                DatabaseContext.SaveChanges();

                // Return response
                return true;
            }

            // Return response
            return false;
        }
    }
}
