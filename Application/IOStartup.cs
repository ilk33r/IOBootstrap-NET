using System;
using IOBootstrap.NET.Application.Filters;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Middlewares;
using IOBootstrap.NET.Common.Routes;
using IOBootstrap.NET.Core.Middlewares;
using IOBootstrap.NET.Core.Services.Captcha;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IOBootstrap.NET.Application;

public abstract class IOStartup<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
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
        long maxUploadRequestSize = GetMaxUploadRequestSize();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = maxUploadRequestSize;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxUploadRequestSize;
        });

        services.AddDbContext<TDBContext>(opt => DatabaseContextOptions((DbContextOptionsBuilder<TDBContext>)opt));
        services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<IODatabaseLogFilter<TDBContext>>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

        if (UseSwagger())
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // builder.Services.AddEndpointsApiExplorer();

            string authorization = Configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey)!;
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

        services.AddDistributedMemoryCache();
        services.AddHealthChecks();

        string? dataProtectionPath = Configuration.GetValue<string>(IOConfigurationConstants.DataProtectionPath);
        if (!String.IsNullOrEmpty(dataProtectionPath))
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
                .SetApplicationName("io-app");
        }

        services.AddSession(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.Name = ".IO.Session";
        });

        string[] allowedOrigins = Configuration.GetSection(IOConfigurationConstants.AllowedOrigins).Get<string[]>()!;
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                foreach (string allowedOrigin in allowedOrigins)
                {
                    builder.WithOrigins(allowedOrigin);
                }

                builder.WithExposedHeaders(IORequestHeaderConstants.Nonce);
                builder.WithExposedHeaders(IORequestHeaderConstants.SessionID);
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.AllowCredentials();
                builder.Build();
            });
        });
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddSingleton<IWebHostEnvironment>(Environment);

        services.AddCaptcha(x =>
        {
            x.FontFamilies = [
                "AncizarSerif-VariableFont.ttf",
                "OpenSans-VariableFont.ttf",
                "Oswald-VariableFont.ttf"
            ];
            x.DrawLines = 5;
        });
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<IOLoggerType> logger)
    {
        // Use static files
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=640800");
            }
        });

        // Swagger
        if (UseSwagger())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger(options =>
            {
                options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "IOBootstrapt");
                options.RoutePrefix = "swagger-ui";
                options.ConfigObject.TryItOutEnabled = true;
                options.InjectJavascript("/swagger-ui/swagger-encryption.js");
                options.InjectJavascript("/swagger-ui/swagger-encryption-interceptor.js");
            });
        }

        // Use session
        app.UseSession();

        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering();
            await next();
        });

        // Use middleware
        ConfigureMiddleWare(app, env, logger);

        string indexControllerName = Configuration.GetValue<string>(IOConfigurationConstants.IndexControllerNameKey)!;
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
            endpoints.MapControllerRoute("", indexRoute.GetRouteString());
            endpoints.MapControllers();
            endpoints.MapControllerRoute("Error404", errorRoute.GetRouteString());
        });

        app.UseHealthChecks("/Health");

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

    public virtual bool UseSwagger() 
    {
        return Environment.IsDevelopment() || Environment.IsStaging();
    } 

    private long GetMaxUploadRequestSize()
    {
        long maxFileSize = Configuration.GetValue<long>(IOConfigurationConstants.IOMaxUploadFileSize);
        if (maxFileSize <= 0)
        {
            return 128L * 1024L * 1024L;
        }

        return maxFileSize + (5L * 1024L * 1024L);
    }

    public virtual void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.OperationFilter<IODefaultHeaderFilter>();
    }

    public virtual void ConfigureMiddleWare(IApplicationBuilder app, IWebHostEnvironment env, ILogger<IOLoggerType> logger)
    {
        app.UseMiddleware(typeof(IOErrorHandlingMiddleware<TDBContext>));
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
