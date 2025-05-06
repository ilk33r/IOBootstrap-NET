#if DEBUG
using System.Threading.Tasks;
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
public class IODatabaseContentGeneratorController<TDBContext> : IOController<IODatabaseContentGeneratorViewModel<TDBContext>, TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
{
    public IODatabaseContentGeneratorController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    [HttpGet("[action]")]
    public async Task<IOResponseModel> CreateBOUser([FromQuery] string userName)
    {
        await ViewModel.CreateBOUser(userName);
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