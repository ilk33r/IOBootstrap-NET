using System;
using IOBootstrap.NET.BackOffice.BackOffice.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.BackOffice.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeDefaultController : IOBackOfficeController<IOBackOfficeDefaultViewModel>
    {
        public IOBackOfficeDefaultController(IConfiguration configuration, 
                                             IWebHostEnvironment environment, 
                                             ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
