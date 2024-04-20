using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers;

[EnableCors]
[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[IOSessionAttribute]
public class IOHandshakeDefaultController : IOHandshakeController<IOHandshakeDefaultViewModel, IODatabaseContextDefaultImpl>
{
    public IOHandshakeDefaultController(IConfiguration configuration,
                                        IWebHostEnvironment environment,
                                        ILogger<IOLoggerType> logger,
                                        IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }
}
