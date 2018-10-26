using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddLogging();
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
            // Add console logger
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Use session
            app.UseSession();

            // Log
            bool useDeveloperLog = this.Configuration.GetValue<bool>("IOUseDeveloperLog");
            if (useDeveloperLog)
            {
                app.UseDeveloperExceptionPage();   
            }

            // Create default routes
            app.UseMvc(routes =>
            {
                routes.MapRoute("authenticate", "backoffice/users/password/authenticate", new IORoute("Authenticate", this.AuthenticationControllerName()));
                routes.MapRoute("checktoken", "backoffice/users/password/checktoken", new IORoute("CheckToken", this.AuthenticationControllerName()));
                routes.MapRoute("addClient", "backoffice/clients/add", new IORoute("AddClient", this.BackOfficeControllerName()));
                routes.MapRoute("deleteClient", "backoffice/clients/delete", new IORoute("DeleteClient", this.BackOfficeControllerName()));
                routes.MapRoute("listClient", "backoffice/clients/list", new IORoute("ListClients", this.BackOfficeControllerName()));
                routes.MapRoute("updateClient", "backoffice/clients/update", new IORoute("UpdateClient", this.BackOfficeControllerName()));
                routes.MapRoute("addUser", "backoffice/users/add", new IORoute("AddUser", this.UserControllerName()));
                routes.MapRoute("changePassword", "backoffice/users/password/change", new IORoute("ChangePassword", this.UserControllerName()));
                routes.MapRoute("deleteUser", "backoffice/users/delete", new IORoute("DeleteUser", this.UserControllerName()));
                routes.MapRoute("listUsers", "backoffice/users/list", new IORoute("ListUsers", this.UserControllerName()));
                routes.MapRoute("updateUsers", "backoffice/users/update", new IORoute("UpdateUser", this.UserControllerName()));
                routes.MapRoute("addMenuItem", "backoffice/menu/add", new IORoute("AddMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("listMenuItems", "backoffice/menu/list", new IORoute("ListMenuItems", this.BackOfficeMenuControllerName()));
                routes.MapRoute("updateMenuItem", "backoffice/menu/update", new IORoute("UpdateMenuItem", this.BackOfficeMenuControllerName()));
                routes.MapRoute("addMessagesItem", "backoffice/messages/add", new IORoute("AddMessagesItem", this.BackOfficeMessagesControllerName()));
                routes.MapRoute("listMessagesItems", "backoffice/messages/list", new IORoute("ListMessages", this.BackOfficeMessagesControllerName()));
#if DEBUG
                routes.MapRoute("generateKeys", "generate/keys", new IORoute("GenerateKeys", this.KeyControllerName()));
#endif
                routes.MapRoute("default", "", new IORoute("Index", this.BaseControllerName()));
                routes.MapRoute("Error404", "{*url}", new IORoute("Error404", this.BaseControllerName()));
            });
        }

#endregion

#region Routing

        public virtual string AuthenticationControllerName()
        {
            return "IOAuthentication";
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

        public virtual string KeyControllerName() 
        {
            return "IOKeyGenerator";
        }

        public virtual string UserControllerName()
        {
            return "IOUser";
        }

#endregion
    }
}
