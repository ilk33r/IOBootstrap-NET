using System.Text.Json;
using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.Batch.Base.Program;

public class BatchProgram<TConfig>
{

    public static string? Environment { get; set; }
    public static ILogger<IOLoggerType>? Logger { get; set; }
    public static TConfig? Configuration { get; set; }

    public static void Initialize(string[] args)
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
    }

    public static string CurrentDirectory()
    {
        return Directory.GetCurrentDirectory();

        // Microsoft.Azure.WebJobs.ExecutionContext context = new Microsoft.Azure.WebJobs.ExecutionContext();
        // context.FunctionDirectory = Directory.GetCurrentDirectory() + "/bin";
    }
}
