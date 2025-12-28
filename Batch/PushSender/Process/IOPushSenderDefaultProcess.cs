using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Process;

public class IOPushSenderDefaultProcess : IOPushSenderProcess<IOBatchConfigurationModel, IODatabaseContextDefaultImpl, IOPushNotificationDevicesDefaultEntity>
{
}