using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOBackOfficeViewModel<TDBContext> : IIOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
{
    #region Publics

    public IOUserInfoModel UserModel { get; set; }

    #endregion
        
    #region View Model Methods

    public IOClientInfoModel CreateClient(IOClientAddRequestModel requestModel);

    public void DeleteClient(IOClientDeleteRequestModel requestModel);

    public IList<IOClientInfoModel> GetClients();

    public void UpdateClient(IOClientUpdateRequestModel requestModel);
    
    public bool IsBackOffice();

    #endregion
}
