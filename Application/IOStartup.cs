using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.Core.Middlewares;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.EntityFrameworkCore;

namespace IOBootstrap.NET.Application
{
    public abstract class IOStartup<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        #endregion

        #region Initialization Methods

        public IOStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        #endregion

        #region Configurations

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TDBContext>(opt => DatabaseContextOptions((DbContextOptionsBuilder<TDBContext>)opt));
            services.AddDistributedMemoryCache();
            services.AddControllers();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
            services.AddSession(options =>
            {
                options.Cookie.Name = ".IO.Session";
            });
            
            string allowedOrigin = Configuration.GetValue<string>(IOConfigurationConstants.AllowedOrigin);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => 
                {
                    builder.WithOrigins(allowedOrigin).AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IWebHostEnvironment>(Environment);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=640800");
                }
            });

            // Use session
            app.UseSession();

            // Log
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use middleware
			app.UseMiddleware(typeof(IOErrorHandlingMiddleware));

            IORoute errorRoute = new IORoute("Error404", Configuration.GetValue<string>(IOConfigurationConstants.IndexControllerNameKey));
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    context.Items["OriginalPath"] = context.Request.Path.Value;
                    context.Request.Path = "/" + errorRoute.Controller + "/Error404";
                    await next();
                }
            });

            // Use routing 
            app.UseRouting();

            // Use redirection and cors
            app.UseHttpsRedirection();
            app.UseCors();

            IORoute indexRoute = new IORoute("Index", Configuration.GetValue<string>(IOConfigurationConstants.IndexControllerNameKey));
            IORoute generateKeyRoute = new IORoute("GenerateKeys", "IOKeyGenerator");
            
            string backofficeControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IORoute addClientRoute = new IORoute("AddClient", backofficeControllerName);
            IORoute deleteClientRoute = new IORoute("DeleteClient", backofficeControllerName);
            IORoute listClientRoute = new IORoute("ListClients", backofficeControllerName);
            IORoute updateClientRoute = new IORoute("UpdateClient", backofficeControllerName);

            string userControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            IORoute addUserRoute = new IORoute("AddUser", userControllerName);
            IORoute changePasswordRoute = new IORoute("ChangePassword", userControllerName);
            IORoute deleteUserRoute = new IORoute("DeleteUser", userControllerName);
            IORoute listUsersRoute = new IORoute("ListUsers", userControllerName);
            IORoute updateUsersRoute = new IORoute("UpdateUser", userControllerName);

            string authenticationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IORoute authenticateRoute = new IORoute("Authenticate", authenticationControllerName);
            IORoute checkTokenRoute = new IORoute("CheckToken", authenticationControllerName);

            string configurationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IORoute addConfigurationItemRoute = new IORoute("AddConfigItem", configurationControllerName);
            IORoute deleteConfigurationItemRoute = new IORoute("DeleteConfigItem", configurationControllerName);
            IORoute listConfigurationItemsRoute = new IORoute("ListConfigurationItems", configurationControllerName);
            IORoute updateConfigurationItemRoute = new IORoute("UpdateConfigItem", configurationControllerName);
            IORoute restartAppRoute = new IORoute("RestartApp", configurationControllerName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", indexRoute.GetRouteString());
                endpoints.MapControllerRoute("addUser", addUserRoute.GetRouteString());
                endpoints.MapControllerRoute("changePassword", changePasswordRoute.GetRouteString());
                endpoints.MapControllerRoute("deleteUser", deleteUserRoute.GetRouteString());
                endpoints.MapControllerRoute("listUsers", listUsersRoute.GetRouteString());
                endpoints.MapControllerRoute("updateUsers", updateUsersRoute.GetRouteString());
                endpoints.MapControllerRoute("addClient", addClientRoute.GetRouteString());
                endpoints.MapControllerRoute("deleteClient", deleteClientRoute.GetRouteString());
                endpoints.MapControllerRoute("listClient", listClientRoute.GetRouteString());
                endpoints.MapControllerRoute("updateClient", updateClientRoute.GetRouteString());
                endpoints.MapControllerRoute("authenticate", authenticateRoute.GetRouteString());
                endpoints.MapControllerRoute("checktoken", checkTokenRoute.GetRouteString());
                endpoints.MapControllerRoute("addConfigurationItem", addConfigurationItemRoute.GetRouteString());
                endpoints.MapControllerRoute("deleteConfigurationItem", deleteConfigurationItemRoute.GetRouteString());
                endpoints.MapControllerRoute("listConfigurationItems", listConfigurationItemsRoute.GetRouteString());
                endpoints.MapControllerRoute("updateConfigurationItem", updateConfigurationItemRoute.GetRouteString());
                endpoints.MapControllerRoute("restartApp", restartAppRoute.GetRouteString());

#if DEBUG
                endpoints.MapControllerRoute("generateKeys", generateKeyRoute.GetRouteString());
#endif
                endpoints.MapControllerRoute("Error404", errorRoute.GetRouteString());
            });

            // Start static caching
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                TDBContext context = services.GetService<TDBContext>();
                ConfigureStaticCaching(context);
            }
        }

        public virtual void ConfigureStaticCaching(TDBContext databaseContext)
        {
        }

        public virtual void DatabaseContextOptions(DbContextOptionsBuilder<TDBContext> options)
        {
            string migrationAssembly = Configuration.GetValue<string>(IOConfigurationConstants.MigrationsAssemblyKey);
#if USE_MYSQL_DATABASE
            options.UseMySQL(this.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
#elif USE_SQLSRV_DATABASE
            options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
#else
            options.UseInMemoryDatabase("IOMemory");
#endif
        }

        /*

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Create default routes
            app.UseMvc(routes =>
            {
                routes.MapRoute("addMenuItem", "backOffice/menu/add", new IORoute("AddMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("deleteMenuItem", "backOffice/menu/delete", new IORoute("DeleteMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("listMenuItems", "backOffice/menu/list", new IORoute("ListMenuItems", this.BackOfficeMenuControllerName()));
                routes.MapRoute("updateMenuItem", "backOffice/menu/update", new IORoute("UpdateMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("addMessagesItem", "backOffice/messages/add", new IORoute("AddMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("deleteMessagesItem", "backOffice/messages/delete", new IORoute("DeleteMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("listAllMessagesItems", "backOffice/messages/listall", new IORoute("ListAllMessages", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("listMessagesItems", "backOffice/messages/list", new IORoute("ListMessages", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("updateMessagesItem", "backOffice/messages/update", new IORoute("UpdateMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("addResource", "backOffice/resources/add", new IORoute("AddResource", this.ResourcesControllerName()));
                routes.MapRoute("deleteResource", "backOffice/resources/delete", new IORoute("DeleteResource", this.ResourcesControllerName()));
                routes.MapRoute("getAllResources", "backOffice/resources/all", new IORoute("GetAllResources", this.ResourcesControllerName()));
                routes.MapRoute("getResources", "backOffice/resources/get", new IORoute("GetResources", this.ResourcesControllerName()));
                routes.MapRoute("updateResource", "backOffice/resources/update", new IORoute("UpdateResource", this.ResourcesControllerName()));
            });
        }

        #endregion

        #region Routing

        public virtual string BackOfficeControllerName()
        {
            return "IOBackOffice";
        }

        public virtual string BackOfficeMenuControllerName()
        {
            return "IOBackOfficeMenu";
        }

        public virtual string BackOfficeMessagesControllerName()
        {
            return "IOBackOfficeMessages";
        }

        public virtual string ResourcesControllerName()
        {
            return "IOBackOfficeResources";
        }
*/
        #endregion
    }
}
