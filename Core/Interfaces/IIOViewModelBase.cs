using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOViewModelBase 
{

    #region Properties

    public IConfiguration Configuration { get; set; }
    public IWebHostEnvironment Environment { get; set; }
    public ILogger<IOLoggerType> Logger { get; set; }
    public HttpRequest Request { get; set; }

    #endregion
}