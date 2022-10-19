using System;
using IOBootstrap.NET.BackOffice.Configuration.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers
{
    [IOBackoffice]
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOBackOfficeConfigurationsDefaultController : IOBackOfficeConfigurationsController<IOBackOfficeConfigurationsDefaultViewModel>
    {
        public IOBackOfficeConfigurationsDefaultController(IConfiguration configuration, 
                                                           IWebHostEnvironment environment, 
                                                           ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
