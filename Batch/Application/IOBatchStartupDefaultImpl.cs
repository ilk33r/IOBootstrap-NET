using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Application
{
    public class IOBatchStartupDefaultImpl : IOBatchStartup<IODatabaseContextDefaultImpl>
    {
        public IOBatchStartupDefaultImpl(string configFilePath, bool development) : base(configFilePath, development)
        {
        }
    }
}
