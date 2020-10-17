using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Application
{
    public class IOBatchStartupDefaultImpl : IOBatchStartup<IODatabaseContextDefaultImpl>
    {
        public IOBatchStartupDefaultImpl(string configFilePath, string environment) : base(configFilePath, environment)
        {
        }
    }
}
