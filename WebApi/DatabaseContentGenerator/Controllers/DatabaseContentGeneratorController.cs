#if DEBUG
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.DatabaseContentGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.DatabaseContentGenerator.Controllers;

// [Obsolete("This Method is Deprecated", false)]
[Produces("application/json")]
[ApiController]
[Route("[controller]")]
public class DatabaseContentGeneratorController<TDBContext> : IOController<DatabaseContentGeneratorViewModel<TDBContext>, TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
{
    public DatabaseContentGeneratorController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    [HttpGet("[action]")]
    public IOResponseModel CreateBOUser([FromQuery] string userName)
    {
        ViewModel.CreateBOUser(userName);
        return new IOResponseModel();
    }

    [HttpGet("[action]")]
    public IOResponseModel CreateBODefaultData()
    {
        ViewModel.CreateBODefaultData();
        return new IOResponseModel();
    }
}
#endif