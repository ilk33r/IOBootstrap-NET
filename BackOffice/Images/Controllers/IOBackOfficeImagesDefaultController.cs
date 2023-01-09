using System;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Images.Controllers
{
    [IOBackoffice]
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOBackOfficeImagesDefaultController : IOBackOfficeImagesController<IOBackOfficeImagesDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeImagesDefaultController(IConfiguration configuration, 
                                                   IWebHostEnvironment environment, 
                                                   ILogger<IOLoggerType> logger,
                                                   IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
