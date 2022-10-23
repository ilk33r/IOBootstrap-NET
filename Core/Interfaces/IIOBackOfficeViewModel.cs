using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOBackOfficeViewModel : IIOViewModel
{

    #region View Model Methods

    public IOClientInfoModel CreateClient(IOClientAddRequestModel requestModel);

    public void DeleteClient(IOClientDeleteRequestModel requestModel);

    public IList<IOClientInfoModel> GetClients();

    public void UpdateClient(IOClientUpdateRequestModel requestModel);
    
    public bool IsBackOffice();

    #endregion
}
