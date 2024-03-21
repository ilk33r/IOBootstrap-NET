using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOViewModel<TDBContext> : IIOViewModelBase where TDBContext : IODatabaseContext<TDBContext>
{
    #region Properties

    public TDBContext DatabaseContext { get; set; }

    #endregion

    #region Helper Methods

    public void CheckAuthorizationHeader();

    public void CheckClient();

    public int GetUserRole();

    #endregion

    #region Configuration

    public IOConfigurationModel GetDBConfig(string configKey);

    #endregion
}
