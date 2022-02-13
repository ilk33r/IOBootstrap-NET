using System;

namespace IOBootstrap.NET.Application
{
    public class Program
    {
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public static IHost BuildWebHost(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                                                                     {
                                                                         webBuilder.UseStartup<IOStartupDefault>();
                                                                     }).Build();

        public static void Main(string[] args)
        {
            BuildWebHost(args).RunAsync(cancelTokenSource.Token).GetAwaiter().GetResult();
        }

        public static void Shutdown()
        {
            cancelTokenSource.Cancel();
        }
    }
}
