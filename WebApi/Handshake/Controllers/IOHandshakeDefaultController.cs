using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;

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
