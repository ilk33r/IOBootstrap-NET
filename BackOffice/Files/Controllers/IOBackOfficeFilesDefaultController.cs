using System;
using IOBootstrap.NET.BackOffice.Files.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Files.Controllers;

[IOBackoffice]
[EnableCors]
[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[IOSession]
public class IOBackOfficeFilesDefaultController : IOBackOfficeFilesController<IOBackOfficeFilesDefaultViewModel, IODatabaseContextDefaultImpl>
{
    public IOBackOfficeFilesDefaultController(IConfiguration configuration,
                                               IWebHostEnvironment environment,
                                               ILogger<IOLoggerType> logger,
                                               IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }
}
