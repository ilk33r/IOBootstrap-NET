using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IOBootstrap.NET.Application
{
    public class Program
    {
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public static IHostBuilder BuildWebHost(string[] args)
		{
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<IOStartup>();
            });
		}

        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().RunAsync(cancelTokenSource.Token).GetAwaiter().GetResult();
        }

        public static void Shutdown()
        {
            cancelTokenSource.Cancel();
        }
    }
}
