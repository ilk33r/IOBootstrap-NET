using System;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.Application
{
    public abstract class IOBatchStartup<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        protected bool IsDevelopment { get; set; }
        protected IConfiguration Configuration { get; set; }
        protected string ConfigurationPath { get; set; }
        protected TDBContext DatabaseContext { get; set; }
        protected ILogger<IOLoggerType> Logger { get; set; }

        #endregion

        #region Initialization Methods

        public IOBatchStartup(string configFilePath, bool development)
        {
            // Setup properties
            this.IsDevelopment = development;
            this.Configuration = this.GetConfigurations(configFilePath);
            this.ConfigurationPath = configFilePath;
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddDebug()
            );
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("SampleApp.Program", LogLevel.Debug)
                        .AddConsole();
            });
            this.Logger = loggerFactory.CreateLogger<IOLoggerType>();

            // Create database context
            DbContextOptionsBuilder databaseOptionBuilder = new DbContextOptionsBuilder();
            Type databaseContextType = this.GetType().BaseType.GenericTypeArguments[0];
            DbContextOptionsBuilder<TDBContext> databaseOptionsBuilder = new DbContextOptionsBuilder<TDBContext>();
            this.DatabaseContextOptions(databaseOptionsBuilder);
            object[] parameters = new object[] {
                (object)databaseOptionsBuilder.Options
            };
            this.DatabaseContext = (TDBContext)Activator.CreateInstance(this.GetType().BaseType.GenericTypeArguments[0], parameters);

        }

        public void RunBatch(Type batchClass)
        {
            // Create batch class instance
            object[] parameters = new object[] {
                (object)this.IsDevelopment,
                (object)this.Configuration,
                (object)this.ConfigurationPath,
                (object)this.DatabaseContext,
                (object)this.Logger
            };

            IOBatchClass<TDBContext> batchInstance = (IOBatchClass<TDBContext>)Activator.CreateInstance(batchClass, parameters);
            batchInstance.Run();
        }

        public virtual void RunAllBatches()
        {
            // Obtain batch classes
            Type[] batchClasses = this.BatchClasses();

            // Loop Batch Classes
            foreach (Type batchClass in batchClasses)
            {
                this.RunBatch(batchClass);
            }
        }

        #endregion

        #region Helper Methods

        private IConfiguration GetConfigurations(string configFilePath) 
        {
            // Create environment name
            string environmentName;

            // Check environment is development
            if (IsDevelopment)
            {
                environmentName = "Development";
            }
            else
            {
                environmentName = "Production";
            }

            // Create builder
            var builder = new ConfigurationBuilder()
                .SetBasePath(configFilePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // Setup properties
            return builder.Build();
        }

        #endregion

        #region Configs

        public virtual Type[] BatchClasses()
        {
            return new Type[] {};
        }

        public virtual void DatabaseContextOptions(DbContextOptionsBuilder<TDBContext> options)
        {
            options.UseInMemoryDatabase("IOMemory");
        }

        #endregion
    }
}
