using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers
{
    public class IOHandshakeDefaultController : IOHandshakeController<IOHandshakeDefaultViewModel>
    {
        public IOHandshakeDefaultController(IConfiguration configuration, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
