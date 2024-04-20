using IOBootstrap.NET.BackOffice.GenerateBOPage.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.GenerateBOPage.Controllers;

[IOBackoffice]
[EnableCors]
[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[IOSessionAttribute]
public class IOGenerateBOPageDefaultController : IOGenerateBOPageController<IOGenerateBOPageDefaultViewModel, IODatabaseContextDefaultImpl>
{
    public IOGenerateBOPageDefaultController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }
}
