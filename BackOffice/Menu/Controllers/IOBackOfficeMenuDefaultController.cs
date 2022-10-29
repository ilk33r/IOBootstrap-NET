using System;
using IOBootstrap.NET.BackOffice.Menu.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers
{
    [IOBackoffice]
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOBackOfficeMenuDefaultController : IOBackOfficeMenuController<IOBackOfficeMenuDefaultViewModel>
    {
        public IOBackOfficeMenuDefaultController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
