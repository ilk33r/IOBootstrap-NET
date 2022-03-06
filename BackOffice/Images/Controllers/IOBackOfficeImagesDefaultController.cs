using System;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.BackOffice.Images.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeImagesDefaultController : IOBackOfficeImagesController<IOBackOfficeImagesDefaultViewModel>
    {
        public IOBackOfficeImagesDefaultController(IConfiguration configuration, 
                                                   IWebHostEnvironment environment, 
                                                   ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
