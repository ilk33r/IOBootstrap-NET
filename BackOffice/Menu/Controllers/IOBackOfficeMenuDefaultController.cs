using System;
using IOBootstrap.NET.BackOffice.Menu.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeMenuDefaultController : IOBackOfficeMenuController<IOBackOfficeMenuDefaultViewModel>
    {
        public IOBackOfficeMenuDefaultController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
