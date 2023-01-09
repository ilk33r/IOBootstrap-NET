using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
{
    #region Properties

    public IConfiguration Configuration { get; set; }
    public IWebHostEnvironment Environment { get; set; }
    public ILogger<IOLoggerType> Logger { get; set; }
    public HttpRequest Request { get; set; }
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
