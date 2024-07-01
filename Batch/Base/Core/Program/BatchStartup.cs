using System.Text.Json;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.Base.Core.Program;

public abstract class BatchStartup<TConfig, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    public BatchStartup(string[] args)
    {
        // Check argument count is correct
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: [env]");
            throw new Exception("Incorrect parameters");
        }

        Environment = args[0];
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("SampleApp.Program", LogLevel.Debug)
                    .AddConsole();
        });
        Logger = loggerFactory.CreateLogger<IOLoggerType>();

        string workingDirectory = CurrentDirectory();
        string configFilePath = Path.Combine(workingDirectory, "appsettings." + Environment + ".json");
        string configurationJson = File.ReadAllText(configFilePath);

        Configuration = JsonSerializer.Deserialize<TConfig>(configurationJson);
        if (Configuration == null)
        {
            throw new Exception("Invalid configuration json");
        }

        // Log call
        Logger?.LogDebug("Batch initialized");

        // Initialize database
        DbContextOptionsBuilder<TDBContext> options = new DbContextOptionsBuilder<TDBContext>();
        DatabaseContextOptions(options);

        // Log call
        Logger?.LogDebug("Database initialized");
    }

    public virtual string CurrentDirectory()
    {
        return Directory.GetCurrentDirectory();

        // Microsoft.Azure.WebJobs.ExecutionContext context = new Microsoft.Azure.WebJobs.ExecutionContext();
        // context.FunctionDirectory = Directory.GetCurrentDirectory() + "/bin";
    }

    public virtual void DatabaseContextOptions(DbContextOptionsBuilder<TDBContext> options)
    {
        options.UseInMemoryDatabase("IOMemory");
    }
}
