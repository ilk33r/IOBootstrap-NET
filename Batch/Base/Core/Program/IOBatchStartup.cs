using System.Diagnostics;
using System.Text.Json;
using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.Base.Core.Interface;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.Base.Core.Program;

public abstract class IOBatchStartup<TConfig, TDBContext, TPushNotificationDevicesEntity>
where TConfig : IOBatchConfigurationModel
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    private ServiceCollection ServiceCollection { get; set; }
    private IServiceProvider ServiceProvider { get; set; }

    private List<IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>> RegisteredProcesses { get; set; }

    public IOBatchStartup(string[] args)
    {
        // Check argument count is correct
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: [env]");
            throw new Exception("Incorrect parameters");
        }

        Environment = args[0];
        ServiceCollection = new ServiceCollection();
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

        // Configure services
        ConfigureServices(ServiceCollection);
        ServiceProvider = ServiceCollection.BuildServiceProvider();
        ConfigureServiceProviders(ServiceProvider);

        // Process Register
        RegisteredProcesses = new List<IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>>();
        Logger?.LogDebug("Process register initialized");
    }

    public virtual void ConfigureServices(IServiceCollection serviceCollection)
    {
        Logger?.LogDebug("Configure services");
    }

    public virtual void ConfigureServiceProviders(IServiceProvider ServiceProvider)
    {
        Logger?.LogDebug("Configure service providers");
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

        IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>? process = (IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>)processInstance;

        if (process == null)
        {
            Logger?.LogError("Could not initialize process name: {0}", processType.Name);
            return;
        }

        process.Environment = Environment;
        process.Logger = Logger;
        process.Configuration = Configuration;
        process.DatabaseContext = DatabaseContext;
        process.ServiceProvider = ServiceProvider;
        RegisteredProcesses.Add(process);

        process.OnLoad();
        Logger?.LogDebug("Process registered: {0}", processType.Name);
    }

    public virtual void Run(int executionCount, int executionDelayMilliseconds)
    {
        if (executionCount > 0)
        {
            for (int i = 0; i < executionCount; i++)
            {
                RunSubProcesses();
                Thread.Sleep(executionDelayMilliseconds);
            }

            return;
        }

        while (true)
        {
            RunSubProcesses();
            Thread.Sleep(executionDelayMilliseconds);
        }
    }

    private void RunSubProcesses()
    {
        foreach (IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> process in RegisteredProcesses)
        {
            try
            {
                Task subProcess = process.Run();
                subProcess.Wait();   
            }
            catch (Exception e)
            {
                IOExceptionEntity exception = LogException(e);
                
                DatabaseContext?.Add(exception);
                DatabaseContext?.SaveChanges();

                Logger?.LogError("Process {0} exception.\n{1}\n\n{2}\n", process.ToString(), e.Message, e.StackTrace?.ToString());
                Thread.Sleep(2000);
            }
        }
    }

    private IOExceptionEntity LogException(Exception ex)
    {
        string requestPath = "IOBatchStartup";
        string exceptionMessage = "";
        if (!String.IsNullOrEmpty(ex.Message))
        {
            exceptionMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 2048));
        }

        string exceptionStackTrace = "";
        if (!String.IsNullOrEmpty(ex.StackTrace))
        {
            exceptionStackTrace = ex.StackTrace?.Substring(0, Math.Min(ex.StackTrace?.Length ?? 0, 2048)) ?? "";
        }

        return new IOExceptionEntity()
        {
            RequestDate = DateTimeOffset.UtcNow,
            RequestPath = requestPath,
            RequestHeaders = string.Empty,
            RequestBody = string.Empty,
            ExceptionMessage = exceptionMessage,
            ExceptionStackTrace = exceptionStackTrace,
        };
    }
}
