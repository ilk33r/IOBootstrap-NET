using IOBootstrap.NET.BackOffice.GenerateBOPage.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

#if DEBUG
namespace IOBootstrap.NET.BackOffice.GenerateBOPage.Controllers;

[IOBackoffice]
[EnableCors]
[Produces("application/octet-stream")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[IOSession]
public class IOGenerateBOPageFilesDefaultController : IOGenerateBOPageFilesController<IOGenerateBOPageFilesDefaultViewModel, IODatabaseContextDefaultImpl>
{
    public IOGenerateBOPageFilesDefaultController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }
}
#endif