using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Base.Core.Interface;

public interface IBatchProcess<TConfig, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; }
    public ILogger<IOLoggerType>? Logger { get; }
    public TConfig? Configuration { get; }
    public TDBContext? DatabaseContext { get; }

    public void Run();
}
