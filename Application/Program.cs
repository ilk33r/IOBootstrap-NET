using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace IOBootstrap.NET.Core.System
{
    public class Program
    {
		public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args).UseStartup<IOStartup>().Build();
		}

        public static void Main(string[] args)
        {
			BuildWebHost(args).Run();
        }
    }
}
