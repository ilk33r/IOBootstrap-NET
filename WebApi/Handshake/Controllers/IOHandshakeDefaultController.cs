using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOHandshakeDefaultController : IOHandshakeController<IOHandshakeDefaultViewModel>
    {
        public IOHandshakeDefaultController(IConfiguration configuration, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
