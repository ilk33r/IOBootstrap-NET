using System;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.Application
{
    public abstract class IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public IConfiguration Configuration { get; set; }
        public String ConfigurationPath { get; set; }
		public TDBContext DatabaseContext { get; set; }
        public string EnvironmentName { get; set; }
        public ILogger<IOLoggerType> Logger { get; set; }

        #endregion

        #region Initialization Methods

        public IOBatchClass(string environment, 
                            IConfiguration configuration,
                            string configurationPath, 
                            TDBContext databaseContext, 
                            ILogger<IOLoggerType> logger)
        {
            // Setup properties
            this.EnvironmentName = environment;
            this.Configuration = configuration;
            this.ConfigurationPath = configurationPath;
            this.DatabaseContext = databaseContext;
            this.Logger = logger;
        }

        public virtual void Run()
        {
        }

        #endregion

    }
}
