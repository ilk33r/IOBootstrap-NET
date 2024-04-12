using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOViewModel<TDBContext>, new()
{

    public IConfiguration Configuration { get; set; }
    public IWebHostEnvironment Environment { get; }
    public ILogger<IOLoggerType> Logger { get; }
    public TViewModel ViewModel { get; }
}
