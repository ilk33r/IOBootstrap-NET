using System;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.User.Controllers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.User.Controllers
{
    [IOBackoffice]
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOBackOfficeUserDefaultController : IOUserController<IOBackOfficeUserDefaultViewModel>
    {
        public IOBackOfficeUserDefaultController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
