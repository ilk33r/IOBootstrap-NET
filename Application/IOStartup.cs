using System.Linq;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Application
{
    public abstract class IOStartup<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment;

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
            Environment = env;
        }

        #endregion

        #region Configurations

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<TDBContext>(opt => this.DatabaseContextOptions((DbContextOptionsBuilder<TDBContext>)opt));
            services.AddDistributedMemoryCache();
            services.AddMvc();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
            services.AddSession(options =>
            {
                options.Cookie.Name = ".IO.Session";
            });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IHostingEnvironment>(Environment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=640800");
                }
            });

            // Use session
            app.UseSession();

            // Log
            bool useDeveloperLog = this.Configuration.GetValue<bool>(IOConfigurationConstants.UseDeveloperLog);
            if (useDeveloperLog)
            {
                app.UseDeveloperExceptionPage();   
            }
			
			// Use middleware
			app.UseMiddleware(typeof(IOErrorHandlingMiddleware));

            // Create default routes
            app.UseMvc(routes =>
            {
                routes.MapRoute("authenticate", "backOffice/users/password/authenticate", new IORoute("Authenticate", this.AuthenticationControllerName()));
                routes.MapRoute("checktoken", "backOffice/users/password/checktoken", new IORoute("CheckToken", this.AuthenticationControllerName()));
                routes.MapRoute("addClient", "backOffice/clients/add", new IORoute("AddClient", this.BackOfficeControllerName()));
                routes.MapRoute("deleteClient", "backOffice/clients/delete", new IORoute("DeleteClient", this.BackOfficeControllerName()));
                routes.MapRoute("listClient", "backOffice/clients/list", new IORoute("ListClients", this.BackOfficeControllerName()));
                routes.MapRoute("updateClient", "backOffice/clients/update", new IORoute("UpdateClient", this.BackOfficeControllerName()));
                routes.MapRoute("addUser", "backOffice/users/add", new IORoute("AddUser", this.UserControllerName()));
                routes.MapRoute("changePassword", "backOffice/users/password/change", new IORoute("ChangePassword", this.UserControllerName()));
                routes.MapRoute("deleteUser", "backOffice/users/delete", new IORoute("DeleteUser", this.UserControllerName()));
                routes.MapRoute("listUsers", "backOffice/users/list", new IORoute("ListUsers", this.UserControllerName()));
                routes.MapRoute("updateUsers", "backOffice/users/update", new IORoute("UpdateUser", this.UserControllerName()));
                routes.MapRoute("addMenuItem", "backOffice/menu/add", new IORoute("AddMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("deleteMenuItem", "backOffice/menu/delete", new IORoute("DeleteMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("listMenuItems", "backOffice/menu/list", new IORoute("ListMenuItems", this.BackOfficeMenuControllerName()));
                routes.MapRoute("updateMenuItem", "backOffice/menu/update", new IORoute("UpdateMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("addMessagesItem", "backOffice/messages/add", new IORoute("AddMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("deleteMessagesItem", "backOffice/messages/delete", new IORoute("DeleteMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("listAllMessagesItems", "backOffice/messages/listall", new IORoute("ListAllMessages", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("listMessagesItems", "backOffice/messages/list", new IORoute("ListMessages", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("updateMessagesItem", "backOffice/messages/update", new IORoute("UpdateMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("addConfigurationItem", "backOffice/configurations/add", new IORoute("AddConfigItem", this.BackOfficeConfigurationControllerName()));
                routes.MapRoute("deleteConfigurationItem", "backOffice/configurations/delete", new IORoute("DeleteConfigItem", this.BackOfficeConfigurationControllerName()));
                routes.MapRoute("listConfigurationItems", "backOffice/configurations/list", new IORoute("ListConfigurationItems", this.BackOfficeConfigurationControllerName()));
                routes.MapRoute("updateConfigurationItem", "backOffice/configurations/update", new IORoute("UpdateConfigItem", this.BackOfficeConfigurationControllerName()));
                routes.MapRoute("restartApp", "backOffice/configurations/restartApp", new IORoute("RestartApp", this.BackOfficeConfigurationControllerName()));
                routes.MapRoute("addResource", "backOffice/resources/add", new IORoute("AddResource", this.ResourcesControllerName()));
                routes.MapRoute("deleteResource", "backOffice/resources/delete", new IORoute("DeleteResource", this.ResourcesControllerName()));
                routes.MapRoute("getAllResources", "backOffice/resources/all", new IORoute("GetAllResources", this.ResourcesControllerName()));
                routes.MapRoute("getResources", "backOffice/resources/get", new IORoute("GetResources", this.ResourcesControllerName()));
                routes.MapRoute("updateResource", "backOffice/resources/update", new IORoute("UpdateResource", this.ResourcesControllerName()));
#if DEBUG
                routes.MapRoute("generateKeys", "generate/keys", new IORoute("GenerateKeys", this.KeyControllerName()));
#endif
                routes.MapRoute("default", "", new IORoute("Index", this.BaseControllerName()));
                routes.MapRoute("Error404", "{*url}", new IORoute("Error404", this.BaseControllerName()));
            });

            // Start static caching
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                TDBContext context = services.GetService<TDBContext>();
                this.ConfigureStaticCaching(context);
            }
        }

        public virtual void ConfigureStaticCaching(TDBContext databaseContext)
        {
        }

        public virtual void DatabaseContextOptions(DbContextOptionsBuilder<TDBContext> options)
        {
#if USE_MYSQL_DATABASE
            options.UseMySql(this.Configuration.GetConnectionString("DefaultConnection"));
#elif USE_SQLSRV_DATABASE
            options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
#else
            options.UseInMemoryDatabase("IOMemory");
#endif
        }

        #endregion

        #region Routing

        public virtual string AuthenticationControllerName()
        {
            return "IOAuthentication";
        }

        public virtual string BackOfficeConfigurationControllerName()
        {
            return "IOBackOfficeConfigurations";
        }

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

        public virtual string BaseControllerName()
        {
            return "IO";
        }

        public virtual string KeyControllerName() 
        {
            return "IOKeyGenerator";
        }

        public virtual string ResourcesControllerName()
        {
            return "IOBackOfficeResources";
        }

        public virtual string UserControllerName()
        {
            return "IOUser";
        }

        #endregion
    }
}
