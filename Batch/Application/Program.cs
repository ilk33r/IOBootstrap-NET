using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Batch.Application
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

            // Start batch
            IOBatchStartupDefaultImpl startup = new IOBatchStartupDefaultImpl(configPath, args[0]);
            startup.RunAllBatches();
        }
    }
}
