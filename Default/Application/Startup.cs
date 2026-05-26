using IOBootstrap.NET.Application;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Default.Application;

public class Startup : IOStartup<IODatabaseContextDefaultImpl>
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
    }

    public override void DatabaseContextOptions(DbContextOptionsBuilder<IODatabaseContextDefaultImpl> options)
    {
        string migrationAssembly = Configuration.GetValue<string>(IOConfigurationConstants.MigrationsAssemblyKey) ?? "";
#if DEBUG
            options.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                .AddConsole();
            }));
            options.EnableSensitiveDataLogging(true);
#endif

#if PLAIN_CONNECTION_STRING
        // Obtain connection string
        string? connectionString = Configuration.GetConnectionString("DefaultConnection");
#elif ENV_CONNECTION_STRING
        // Obtain connection string
        string? connectionString = System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.ConnectionString) ?? string.Empty;
#else
        // Obtain connection string
        string? connectionString = Configuration.GetConnectionString("EncryptedConnection");

        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionKey)!);
        byte[] iv = Convert.FromBase64String(Configuration.GetValue<string>(IOConfigurationConstants.EncryptionIV)!);

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);
        
        // Obtain decrypted token value
        connectionString = aesUtilities.Decrypt(Convert.FromBase64String(connectionString ?? ""));
#endif

#if USE_POSTGRES_DATABASE
        options.UseNpgsql(connectionString, o =>
        {
           o.SetPostgresVersion(18, 0)
           .MigrationsAssembly(migrationAssembly);
        });
#elif USE_MYSQL_DATABASE
            // options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 0, 7)), b => b.MigrationsAssembly(migrationAssembly));
#elif USE_SQLSRV_DATABASE
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationAssembly));
#else
        options.UseInMemoryDatabase("IOMemory");
#endif
    }
}
