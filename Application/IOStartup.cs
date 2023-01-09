using System;
using IOBootstrap.NET.Application.Filters;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Middlewares;
using IOBootstrap.NET.Common.Routes;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IOBootstrap.NET.Application
{
    public abstract class IOStartup<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
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

            if (Environment.IsDevelopment() || Environment.IsStaging())
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                // builder.Services.AddEndpointsApiExplorer();

                string authorization = Configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey);
                IOCacheObject authorizationCache = new IOCacheObject(IOCacheKeys.SwaggerAuthorization, authorization, 0);
                IOCache.CacheObject(authorizationCache);

                services.AddSwaggerGen(options =>
                {
                    ConfigureSwagger(options);
                });
            }

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

            // Swagger
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(options =>
                {
                    options.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "IOBootstrapt");
                    options.RoutePrefix = "swagger-ui";
                });
            }

            // Use middleware
            ConfigureMiddleWare(app, env, logger);

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
                endpoints.MapControllerRoute("Error404", errorRoute.GetRouteString());
            });

            // Start static caching
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                TDBContext? context = services.GetService<TDBContext>();
                if (context != null) 
                {
                    ConfigureStaticCaching(context);
                }
            }
        }

        public virtual void ConfigureSwagger(SwaggerGenOptions options)
        {
            options.OperationFilter<IODefaultHeaderFilter>();
        }

        public virtual void ConfigureMiddleWare(IApplicationBuilder app, IWebHostEnvironment env, ILogger<IOLoggerType> logger)
        {
            app.UseMiddleware(typeof(IOErrorHandlingMiddleware));
            app.UseMiddleware(typeof(IOFNRequestDecryptorMiddleware));
        }

        public virtual void ConfigureStaticCaching(TDBContext databaseContext)
        {
        }

        public virtual void DatabaseContextOptions(DbContextOptionsBuilder<TDBContext> options)
        {
            options.UseInMemoryDatabase("IOMemory");
        }

        #endregion
    }
}
