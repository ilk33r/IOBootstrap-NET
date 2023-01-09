using System;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
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
    public class IOBackOfficeUserDefaultController : IOUserController<IOBackOfficeUserDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeUserDefaultController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger,
                                                 IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
