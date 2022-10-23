using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Common.MWConnector;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOViewModel
{
    #region Properties

    public IConfiguration Configuration { get; set; }
    public IWebHostEnvironment Environment { get; set; }
    public ILogger<IOLoggerType> Logger { get; set; }
    public HttpRequest Request { get; set; }
    public IOMWConnectorProtocol MWConnector { get; set; }

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
