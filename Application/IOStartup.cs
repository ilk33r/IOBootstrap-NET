using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.Core.Middlewares;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
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
            services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });
            services.AddRouting();
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

            string indexControllerName = Configuration.GetValue<string>(IOConfigurationConstants.IndexControllerNameKey);
            IORoute errorRoute = new IORoute("Error404", indexControllerName);
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
            app.UseHttpMethodOverride();
            app.UseRouting();

            // Use redirection and cors
            app.UseHttpsRedirection();
            app.UseCors();

            IORoute indexRoute = new IORoute("Index", indexControllerName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", indexRoute.GetRouteString());
                endpoints.MapControllers();
                ConfigureUserEndpoints(endpoints);
                ConfigureClientEndpoints(endpoints);
                ConfigureAuthenticationEndpoints(endpoints);
                ConfigureConfigurationEndpoints(endpoints);
                ConfigureMenuEndpoints(endpoints);
                ConfigureMessagesEndpoints(endpoints);
                ConfigureResourcesEndpoints(endpoints);
                ConfigureImagesEndpoints(endpoints);
                ConfigureEndpoints(endpoints);
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
            options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
#elif USE_SQLSRV_DATABASE
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
#else
            options.UseInMemoryDatabase("IOMemory");
#endif
        }

        #endregion

        #region Routing

        public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            IORoute generateKeyRoute = new IORoute("GenerateKeys", "IOKeyGenerator");
            endpoints.MapControllerRoute("generateKeys", generateKeyRoute.GetRouteString());
        }

        public virtual void ConfigureAuthenticationEndpoints(IEndpointRouteBuilder endpoints)
        {
            string authenticationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IORoute authenticateRoute = new IORoute("Authenticate", authenticationControllerName);
            IORoute checkTokenRoute = new IORoute("CheckToken", authenticationControllerName);
            endpoints.MapControllerRoute("authenticate", authenticateRoute.GetRouteString());
            endpoints.MapControllerRoute("checktoken", checkTokenRoute.GetRouteString());
        }

        public virtual void ConfigureClientEndpoints(IEndpointRouteBuilder endpoints)
        {
            string backofficeControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            IORoute addClientRoute = new IORoute("AddClient", backofficeControllerName);
            IORoute deleteClientRoute = new IORoute("DeleteClient", backofficeControllerName);
            IORoute listClientRoute = new IORoute("ListClients", backofficeControllerName);
            IORoute updateClientRoute = new IORoute("UpdateClient", backofficeControllerName);
            endpoints.MapControllerRoute("addClient", addClientRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteClient", deleteClientRoute.GetRouteString());
            endpoints.MapControllerRoute("listClient", listClientRoute.GetRouteString());
            endpoints.MapControllerRoute("updateClient", updateClientRoute.GetRouteString());
        }

        public virtual void ConfigureConfigurationEndpoints(IEndpointRouteBuilder endpoints)
        {
            string configurationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IORoute addConfigurationItemRoute = new IORoute("AddConfigItem", configurationControllerName);
            IORoute deleteConfigurationItemRoute = new IORoute("DeleteConfigItem", configurationControllerName);
            IORoute listConfigurationItemsRoute = new IORoute("ListConfigurationItems", configurationControllerName);
            IORoute updateConfigurationItemRoute = new IORoute("UpdateConfigItem", configurationControllerName);
            IORoute restartAppRoute = new IORoute("RestartApp", configurationControllerName);
            endpoints.MapControllerRoute("addConfigurationItem", addConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteConfigurationItem", deleteConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("listConfigurationItems", listConfigurationItemsRoute.GetRouteString());
            endpoints.MapControllerRoute("updateConfigurationItem", updateConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("restartApp", restartAppRoute.GetRouteString());
        }

        public virtual void ConfigureImagesEndpoints(IEndpointRouteBuilder endpoints)
        {
            string imagesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            IORoute saveImagesRoute = new IORoute("SaveImages", imagesControllerName);
            IORoute deleteImagesRoute = new IORoute("DeleteImages", imagesControllerName);
            endpoints.MapControllerRoute("saveImages", saveImagesRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteImages", deleteImagesRoute.GetRouteString());
        }

        public virtual void ConfigureMenuEndpoints(IEndpointRouteBuilder endpoints)
        {
            string menuControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IORoute addMenuItemRoute = new IORoute("AddMenuItem", menuControllerName);
            IORoute deleteMenuItemRoute = new IORoute("DeleteMenuItem", menuControllerName);
            IORoute listMenuItemsRoute = new IORoute("ListMenuItems", menuControllerName);
            IORoute updateMenuItemRoute = new IORoute("UpdateMenuItem", menuControllerName);
            endpoints.MapControllerRoute("addMenuItem", addMenuItemRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteMenuItem", deleteMenuItemRoute.GetRouteString());
            endpoints.MapControllerRoute("listMenuItems", listMenuItemsRoute.GetRouteString());
            endpoints.MapControllerRoute("updateMenuItem", updateMenuItemRoute.GetRouteString());
        }

        public virtual void ConfigureMessagesEndpoints(IEndpointRouteBuilder endpoints)
        {
            string messagesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IORoute addMessagesItemRoute = new IORoute("AddMessagesItem", messagesControllerName);
            IORoute deleteMessagesItemRoute = new IORoute("DeleteMessagesItem", messagesControllerName);
            IORoute listAllMessagesItemsRoute = new IORoute("ListAllMessages", messagesControllerName);
            IORoute listMessagesItemsRoute = new IORoute("ListMessages", messagesControllerName);
            IORoute updateMessagesItemRoute = new IORoute("UpdateMessagesItem", messagesControllerName);
            endpoints.MapControllerRoute("addMessagesItem", addMessagesItemRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteMessagesItem", deleteMessagesItemRoute.GetRouteString());
            endpoints.MapControllerRoute("listAllMessagesItems", listAllMessagesItemsRoute.GetRouteString());
            endpoints.MapControllerRoute("listMessagesItems", listMessagesItemsRoute.GetRouteString());
            endpoints.MapControllerRoute("updateMessagesItem", updateMessagesItemRoute.GetRouteString());
        }

        public virtual void ConfigureResourcesEndpoints(IEndpointRouteBuilder endpoints)
        {
            string resourcesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeResourcesControllerNameKey);
            IORoute addResourceRoute = new IORoute("AddResource", resourcesControllerName);
            IORoute deleteResourceRoute = new IORoute("DeleteResource", resourcesControllerName);
            IORoute getAllResourcesRoute = new IORoute("GetAllResources", resourcesControllerName);
            IORoute getResourcesRoute = new IORoute("GetResources", resourcesControllerName);
            IORoute updateResourceRoute = new IORoute("UpdateResource", resourcesControllerName);
            endpoints.MapControllerRoute("addResource", addResourceRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteResource", deleteResourceRoute.GetRouteString());
            endpoints.MapControllerRoute("getAllResources", getAllResourcesRoute.GetRouteString());
            endpoints.MapControllerRoute("getResources", getResourcesRoute.GetRouteString());
            endpoints.MapControllerRoute("updateResource", updateResourceRoute.GetRouteString());
        }

        public virtual void ConfigureUserEndpoints(IEndpointRouteBuilder endpoints)
        {
            string userControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            IORoute addUserRoute = new IORoute("AddUser", userControllerName);
            IORoute changePasswordRoute = new IORoute("ChangePassword", userControllerName);
            IORoute deleteUserRoute = new IORoute("DeleteUser", userControllerName);
            IORoute listUsersRoute = new IORoute("ListUsers", userControllerName);
            IORoute updateUsersRoute = new IORoute("UpdateUser", userControllerName);
            endpoints.MapControllerRoute("addUser", addUserRoute.GetRouteString());
            endpoints.MapControllerRoute("changePassword", changePasswordRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteUser", deleteUserRoute.GetRouteString());
            endpoints.MapControllerRoute("listUsers", listUsersRoute.GetRouteString());
            endpoints.MapControllerRoute("updateUsers", updateUsersRoute.GetRouteString());
        }

        #endregion
    }
}
