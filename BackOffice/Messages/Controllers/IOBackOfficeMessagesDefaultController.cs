using System;
using IOBootstrap.NET.BackOffice.Messages.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Messages.Controllers
{
    [IOBackoffice]
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOBackOfficeMessagesDefaultController : IOBackOfficeMessagesController<IOBackOfficeMessagesDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeMessagesDefaultController(IConfiguration configuration, 
                                                     IWebHostEnvironment environment, 
                                                     ILogger<IOLoggerType> logger,
                                                     IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
