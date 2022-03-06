using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Middlewares;
using IOBootstrap.NET.Common.Routes;

namespace IOBootstrap.NET.Application
{
    public abstract class IOStartup
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
            services.AddDistributedMemoryCache();
            services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();

            services.AddRouting();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddIOFileLogger(options => 
                {
                    Configuration.GetSection("Logging").GetSection("IOFileLogger").GetSection("Options").Bind(options);
                });
            });
            services.AddSession(options =>
            {
                options.Cookie.Name = ".IO.Session";
            });

            string[] allowedOrigins = Configuration.GetSection(IOConfigurationConstants.AllowedOrigins).Get<string[]>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    foreach (string allowedOrigin in allowedOrigins)
                    {
                        builder.WithOrigins(allowedOrigin).AllowAnyMethod().AllowAnyHeader();
                    }
                });
            });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IWebHostEnvironment>(Environment);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<IOLoggerType> logger)
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
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI();
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
                ConfigureHandshakeEndpoints(endpoints);
                ConfigureMenuEndpoints(endpoints);
                ConfigureMessagesEndpoints(endpoints);
                ConfigurePushNotificationEndpoints(endpoints);
                ConfigureImagesEndpoints(endpoints);
                ConfigureEndpoints(endpoints);
                endpoints.MapControllerRoute("Error404", errorRoute.GetRouteString());
            });
        }

        #endregion

        #region Routing

        public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            IORoute generateKeyRoute = new IORoute("GenerateKeys", "IOKeyGenerator");
            endpoints.MapControllerRoute("generateKeys", generateKeyRoute.GetRouteString());

            IORoute encryptRoute = new IORoute("Encrypt", "IOKeyGenerator");
            endpoints.MapControllerRoute("encrypt", encryptRoute.GetRouteString());
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
            IORoute resetCacheRoute = new IORoute("ResetCache", configurationControllerName);
            endpoints.MapControllerRoute("addConfigurationItem", addConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("deleteConfigurationItem", deleteConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("listConfigurationItems", listConfigurationItemsRoute.GetRouteString());
            endpoints.MapControllerRoute("updateConfigurationItem", updateConfigurationItemRoute.GetRouteString());
            endpoints.MapControllerRoute("resetCache", resetCacheRoute.GetRouteString());
        }

        public virtual void ConfigureHandshakeEndpoints(IEndpointRouteBuilder endpoints)
        {
            string handshakeControllerName = Configuration.GetValue<string>(IOConfigurationConstants.HandshakeControllerName);
            IORoute indexRoute = new IORoute("Index", handshakeControllerName);
            IORoute checkSessionRoute = new IORoute("CheckSession", handshakeControllerName);
            endpoints.MapControllerRoute("index", indexRoute.GetRouteString());
            endpoints.MapControllerRoute("checkSession", checkSessionRoute.GetRouteString());
        }

        public virtual void ConfigureImagesEndpoints(IEndpointRouteBuilder endpoints)
        {
            string imagesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            IORoute getImagesRoute = new IORoute("GetImages", imagesControllerName);
            IORoute saveImagesRoute = new IORoute("SaveImages", imagesControllerName);
            IORoute deleteImagesRoute = new IORoute("DeleteImages", imagesControllerName);
            endpoints.MapControllerRoute("getImages", getImagesRoute.GetRouteString());
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

        public virtual void ConfigurePushNotificationEndpoints(IEndpointRouteBuilder endpoints)
        {
            string pushNotificationBOControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePushNotificationControllerNameKey);
            IORoute deleteMessageRoute = new IORoute("DeleteMessage", pushNotificationBOControllerName);
            IORoute listMessagesRoute = new IORoute("ListMessages", pushNotificationBOControllerName);
            IORoute sendNotificationRoute = new IORoute("SendNotification", pushNotificationBOControllerName);
            endpoints.MapControllerRoute("deleteMessage", deleteMessageRoute.GetRouteString());
            endpoints.MapControllerRoute("listMessages", listMessagesRoute.GetRouteString());
            endpoints.MapControllerRoute("sendNotification", sendNotificationRoute.GetRouteString());

            string pushNotificationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.PushNotificationControllerNameKey);
            IORoute addPushNotificationTokenRoute = new IORoute("AddPushNotificationToken", pushNotificationControllerName);
            endpoints.MapControllerRoute("addPushNotificationToken", addPushNotificationTokenRoute.GetRouteString());
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
