using IOBootstrap.NET.Application;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Default.Application
{
    public class Startup : IOStartup<IODatabaseContextDefaultImpl>
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        public override void DatabaseContextOptions(DbContextOptionsBuilder<IODatabaseContextDefaultImpl> options)
        {
            string migrationAssembly = Configuration.GetValue<string>(IOConfigurationConstants.MigrationsAssemblyKey);
            #if DEBUG
            options.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                .AddConsole();
            }));
            options.EnableSensitiveDataLogging(true);
            #endif

            #if USE_MYSQL_DATABASE
            // options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(5, 0, 7)), b => b.MigrationsAssembly(migrationAssembly));
            #elif USE_SQLSRV_DATABASE
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationAssembly));
            #else
            options.UseInMemoryDatabase("IOMemory");
            #endif
        }
    }
}
