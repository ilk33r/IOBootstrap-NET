using System;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IOBootstrap.NET.Application
{
    public class Program
    {
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args).UseStartup<IOStartup>().Build();
		}

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
