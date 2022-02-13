using System;
using IOBootstrap.NET.BackOffice.Configuration.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeConfigurationsDefaultController : IOBackOfficeConfigurationsController<IOBackOfficeConfigurationsDefaultViewModel>
    {
        public IOBackOfficeConfigurationsDefaultController(IConfiguration configuration, 
                                                           IWebHostEnvironment environment, 
                                                           ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
