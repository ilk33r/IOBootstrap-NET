using System;
using IOBootstrap.NET.BackOffice.Messages.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.BackOffice.Messages.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeMessagesDefaultController : IOBackOfficeMessagesController<IOBackOfficeMessagesDefaultViewModel>
    {
        public IOBackOfficeMessagesDefaultController(IConfiguration configuration, 
                                                     IWebHostEnvironment environment, 
                                                     ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
