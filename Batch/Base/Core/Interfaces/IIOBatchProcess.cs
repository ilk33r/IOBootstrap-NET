using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Base.Core.Interface;

public interface IIOBatchProcess<TConfig, TDBContext>
where TConfig : IOBatchConfigurationModel
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    public void OnLoad();
    
    public Task Run();
}
