using System;
using IOBootstrap.NET.Batch.PushNotifications;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Application
{
    public class IOBatchStartupDefaultImpl : IOBatchStartup<IODatabaseContextDefaultImpl>
    {
        public IOBatchStartupDefaultImpl(string configFilePath, string environment) : base(configFilePath, environment)
        {
        }

        public override Type[] BatchClasses()
        {
            return new Type[] {
                typeof(IOPushNotificationBatch<IODatabaseContextDefaultImpl>)
            };
        }
    }
}
