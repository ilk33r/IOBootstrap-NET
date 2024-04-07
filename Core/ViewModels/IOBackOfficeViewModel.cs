using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Core.ViewModels;

public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext>, IIOBackOfficeViewModel<TDBContext>, IIOUserCredential<TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{

    #region Publics

    public IOUserInfoModel? UserModel { get; set; }

    #endregion

    #region Initialization Methods

    public IOBackOfficeViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public virtual IOClientInfoModel CreateClient(IOClientAddRequestModel requestModel)
    {
        // Create a client entity
        IOClientsEntity clientEntity = new IOClientsEntity()
        {
            ClientId = IORandomUtilities.GenerateGUIDString(),
            ClientSecret = IORandomUtilities.GenerateGUIDString(),
            ClientDescription = requestModel.ClientDescription,
            IsEnabled = 1,
            RequestCount = 0,
            MaxRequestCount = requestModel.RequestCount
        };

        // Write client to database
        DatabaseContext.Clients.Add(clientEntity);
        DatabaseContext.SaveChanges();

        // Create and return client info
        return new IOClientInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret, clientEntity.ClientDescription ?? "", 1, 0, clientEntity.MaxRequestCount);
    }

    public void DeleteClient(IOClientDeleteRequestModel requestModel)
    {
        IOClientsEntity? clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

        // Check client entity is not null
        if (clientEntity == null)
        {
            throw new IOInvalidClientException("Client not found.");
        }

        // Delete all entity
        DatabaseContext.Remove(clientEntity);
        DatabaseContext.SaveChanges();
    }

    public IList<IOClientInfoModel> GetClients()
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
                                            client.ClientId ?? "",
                                            client.ClientSecret ?? "",
                                            client.ClientDescription ?? "",
                                            client.IsEnabled,
                                            client.RequestCount,
                                            client.MaxRequestCount);
            });
        }

        // Return clients
        return clientInfos;
    }

    public void UpdateClient(IOClientUpdateRequestModel requestModel)
    {
        // Obtain client entity
        IOClientsEntity? clientEntity = DatabaseContext.Clients.Find(requestModel.ClientId);

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
            return;
        }

        // Return response
        throw new IOInvalidClientException("Client not found.");
    }

    public virtual bool IsBackOffice()
    {
        return this.CheckHasUserTokenAndIsValid();
    }

    #endregion

    #region Helper Methods

    public override int GetUserRole()
    {
        // Check user exists
        if (UserModel != null)
        {
            // Return role
            return UserModel.UserRole;
        }

        return (int)UserRoles.AnonmyMouse;
    }

    #endregion

}
