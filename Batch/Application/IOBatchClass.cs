using IOBootstrap.NET.Core.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace IOBootstrap.NET.Batch.Application
{
    public abstract class IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public bool _isDevelopment { get; set; }
        public IConfiguration _configuration { get; set; }
        public String _configurationPath { get; set; }
		public TDBContext _databaseContext { get; set; }
        public ILogger _logger { get; set; }

        #endregion

        #region Initialization Methods

        public IOBatchClass(bool isDevelopment, 
                            IConfiguration configuration,
                            string configurationPath, 
                            TDBContext databaseContext, 
                            ILogger logger)
        {
            // Setup properties
            this._isDevelopment = isDevelopment;
            this._configuration = configuration;
            this._configurationPath = configurationPath;
            this._databaseContext = databaseContext;
            this._logger = logger;
        }

        public virtual void Run()
        {
        }

        #endregion

    }
}
