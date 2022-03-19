using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Functions;

namespace IOBootstrap.NET.Default.Functions
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Check argument count is correct
            if (args.Length != 2) {
                Console.WriteLine("Incorrect parameters");
                Console.WriteLine("Usage: [env] [config path]");
                return;
            }

            // Obtain is arguments
            string configPath = args[1];

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("SampleApp.Program", LogLevel.Debug)
                        .AddConsole();
            });
            ILogger<IOLoggerType> logger = loggerFactory.CreateLogger<IOLoggerType>();

            // Start batch
            PushNotifications.Run(null, logger);
        }
    }
}
