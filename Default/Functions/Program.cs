using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Functions;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Default.Functions
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Check argument count is correct
            if (args.Length != 1) {
                Console.WriteLine("Incorrect parameters");
                Console.WriteLine("Usage: [env]");
                return;
            }

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("SampleApp.Program", LogLevel.Debug)
                        .AddConsole();
            });
            ILogger<IOLoggerType> logger = loggerFactory.CreateLogger<IOLoggerType>();

            Microsoft.Azure.WebJobs.ExecutionContext context = new Microsoft.Azure.WebJobs.ExecutionContext();
            context.FunctionDirectory = Directory.GetCurrentDirectory() + "/bin";

            // Start batch
            PushNotifications.Run(null, context, logger);
        }
    }
}
