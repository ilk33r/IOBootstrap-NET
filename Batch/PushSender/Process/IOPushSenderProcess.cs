using IOBootstrap.NET.Batch.Base.Core.Interface;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.PushSender;

public class IOPushSenderProcess<TConfig, TDBContext> : IIOBatchProcess<TConfig, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    public virtual void Run()
    {
        throw new NotImplementedException();
    }
}
