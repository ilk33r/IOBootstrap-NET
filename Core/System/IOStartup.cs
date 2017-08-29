using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.AutoIncrements;

namespace IOBootstrap.NET.Core.System
{
    public abstract class IOStartup
    {

        #region Properties

        public IConfigurationRoot Configuration { get; }
        public IIODatabase Database { get; }

        #endregion

        #region Initialization Methods

        public IOStartup(IHostingEnvironment env)
        {
			// Create builder
			var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

			// Setup properties
			Configuration = builder.Build();
            Database = new IODatabase(env.ContentRootPath + Configuration.GetValue<string>("IODatabasePath"));
        }

        #endregion

        #region Configurations

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDistributedMemoryCache();
            services.AddMvc();
            services.AddLogging();
			services.AddSession(options =>
			{
				options.Cookie.Name = ".IO.Session";
			});
            services.AddSingleton<IConfiguration>(Configuration);

            Database.SetDatabaseObjects(this.DatabaseObjects());
            services.AddSingleton<IIODatabase>(Database);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Add console logger
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

			// Use session
			app.UseSession();

            // Create default routes
            app.UseMvc(routes =>
            {
				routes.MapRoute("addClient", "backoffice/clients/add", new IORoute("AddClient", this.BackOfficeControllerName()));
                routes.MapRoute("deleteClient", "backoffice/clients/delete", new IORoute("DeleteClient", this.BackOfficeControllerName()));
				routes.MapRoute("listClient", "backoffice/clients/list", new IORoute("ListClients", this.BackOfficeControllerName()));
				routes.MapRoute("default", "", new IORoute("Index", this.BaseControllerName()));
				routes.MapRoute("Error404", "{*url}", new IORoute("Error404", this.BaseControllerName()));
            });
        }

		#endregion

		#region Database

        public virtual Type[] DatabaseObjects()
		{
			return new Type[] {
				typeof(IOAutoIncrementsEntity),
				typeof(IOClientsEntity)
			};
		}

		#endregion


		#region Routing

		public virtual string BackOfficeControllerName()
		{
			return "IOBackOffice";
		}

		public virtual string BaseControllerName()
		{
			return "IO";
		}

        #endregion
    }
}
