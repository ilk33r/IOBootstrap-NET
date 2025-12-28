using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.Base.Core.Interface;

public interface IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>
where TConfig : IOBatchConfigurationModel
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    public void OnLoad();
    
    public Task Run();
}
