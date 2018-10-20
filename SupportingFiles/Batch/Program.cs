using System;

namespace BoomAppBatch.Application
{
    class Program
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
            bool development = (args[0].Equals("dev")) ? true : false;
            string configPath = args[1];

            // Start batch
            IOBatchStartup startup = new IOBatchStartup(configPath, development);
            startup.RunAllBatches();
        }
    }
}
