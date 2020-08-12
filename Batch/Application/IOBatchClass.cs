﻿using System;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.Application
{
    public abstract class IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public bool IsDevelopment { get; set; }
        public IConfiguration Configuration { get; set; }
        public String ConfigurationPath { get; set; }
		public TDBContext DatabaseContext { get; set; }
        public ILogger<IOLoggerType> Logger { get; set; }

        #endregion

        #region Initialization Methods

        public IOBatchClass(bool isDevelopment, 
                            IConfiguration configuration,
                            string configurationPath, 
                            TDBContext databaseContext, 
                            ILogger<IOLoggerType> logger)
        {
            // Setup properties
            this.IsDevelopment = isDevelopment;
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
