using System;
using System.Linq;
using System.Threading;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IOBootstrap.NET.Application
{
    public class Program
    {
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public static IHost BuildWebHost(string[] args)
		{
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<IOStartup<IODatabaseContextDefaultImpl>>();
            }).Build();
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
