using System;
using IOBootstrap.NET.Application;

namespace IOBootstrap.NET.Default.Application
{
    public class Startup : IOStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints) 
        {
        }
    }
}
