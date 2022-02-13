using System;
using IOBootstrap.NET.BackOffice.Authentication.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers
{
    [IOBackoffice]
    [EnableCors]
    public class IOBackOfficeAuthenticationDefaultController : IOAuthenticationController<IOBackOfficeAuthenticationDefaultViewModel>
    {
        public IOBackOfficeAuthenticationDefaultController(IConfiguration configuration, 
                                                           IWebHostEnvironment environment, 
                                                           ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
