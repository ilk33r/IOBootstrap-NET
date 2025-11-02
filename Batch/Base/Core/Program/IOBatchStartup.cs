using System.Text.Json;
using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.Base.Core.Interface;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.Base.Core.Program;

public abstract class IOBatchStartup<TConfig, TDBContext>
where TConfig : IOBatchConfigurationModel
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    private List<IIOBatchProcess<TConfig, TDBContext>> RegisteredProcesses { get; set; }

    public IOBatchStartup(string[] args)
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
                    .AddFilter(typeof(IOLoggerType).FullName, LogLevel.Debug)
                    .AddConsole();
        });
        Logger = loggerFactory.CreateLogger<IOLoggerType>();

        string workingDirectory = CurrentDirectory();
        string configFilePath = Path.Combine(workingDirectory, "batchsettings." + Environment + ".json");
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

        // Process Register
        RegisteredProcesses = new List<IIOBatchProcess<TConfig, TDBContext>>();
        Logger?.LogDebug("Process register initialized");
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

    public virtual void RegisterProcess(Type processType)
    {
        var processInstance = Activator.CreateInstance(processType);
        if (processInstance == null)
        {
            Logger?.LogError("Could not initialize process name: {0}", processType.Name);
            return;
        }

        IIOBatchProcess<TConfig, TDBContext>? process = (IIOBatchProcess<TConfig, TDBContext>)processInstance;

        if (process == null)
        {
            Logger?.LogError("Could not initialize process name: {0}", processType.Name);
            return;
        }

        process.Environment = Environment;
        process.Logger = Logger;
        process.Configuration = Configuration;
        process.DatabaseContext = DatabaseContext;
        RegisteredProcesses.Add(process);

        process.OnLoad();
        Logger?.LogDebug("Process registered: {0}", processType.Name);
    }

    public virtual void Run()
    {
        while (true)
        {
            RunSubProcesses();
            Thread.Sleep(2000);
        }
    }

    private void RunSubProcesses()
    {
        foreach (IIOBatchProcess<TConfig, TDBContext> process in RegisteredProcesses)
        {
            process.Run();
        }
    }
}
