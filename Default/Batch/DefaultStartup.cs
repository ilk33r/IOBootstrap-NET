using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.Base.Core.Program;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Default.Batch;

public class DefaultStartup : IOBatchStartup<IOBatchConfigurationModel, IODatabaseContextDefaultImpl>
{
    public DefaultStartup(string[] args) : base(args)
    {
    }

    public override void DatabaseContextOptions(DbContextOptionsBuilder<IODatabaseContextDefaultImpl> options)
    {
        #if DEBUG
        options.UseLoggerFactory(LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
            .AddConsole();
        }));
        options.EnableSensitiveDataLogging(true);
        #endif

        #if USE_MYSQL_DATABASE
        options.UseMySql(Configuration?.IOConnectionStrings, new MySqlServerVersion(new Version(5, 0, 7)));
        #endif

        DatabaseContext = new IODatabaseContextDefaultImpl(options.Options);
    }
}
