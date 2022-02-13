using System;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.User.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.User.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeUserDefaultController : IOUserController<IOBackOfficeUserDefaultViewModel>
    {
        public IOBackOfficeUserDefaultController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
