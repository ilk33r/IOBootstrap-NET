﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace IOBootstrap.NET.Core.System
{
    public abstract class IOStartup
    {

        #region Properties

        public IConfigurationRoot Configuration { get; }

        #endregion

        #region Initialization Methods

        public IOStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
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
				options.CookieName = ".IO.Session";
			});
            services.AddSingleton<IConfiguration>(Configuration);
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
                routes.MapRoute("default", "", new { controller = this.BaseControllerName(), action = "Index" });
                routes.MapRoute("Error404", "{*url}", new { controller = this.BaseControllerName(), action = "Error404" });
            });
        }

		#endregion

		#region Routing

        public virtual string BaseControllerName()
		{
            return "IO";
		}

        #endregion
    }
}
