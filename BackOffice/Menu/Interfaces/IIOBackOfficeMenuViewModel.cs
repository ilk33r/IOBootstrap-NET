using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Menu.Interfaces;

public interface IIOBackOfficeMenuViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    public void AddMenuItem(IOMenuAddRequestModel requestModel);

    public void DeleteMenuItem(int menuId);

    public IList<IOMenuListModel> GetMenuTree(int requiredRole);

    public void UpdateMenuItem(IOMenuUpdateRequestModel requestModel);
}
