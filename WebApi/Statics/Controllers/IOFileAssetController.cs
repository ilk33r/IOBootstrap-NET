using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Statics;

public class IOFileAssetController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IOFileAssetViewModel<TDBContext>, new()
{
    public IOFileAssetController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 60)]
    [IOUserRole(UserRoles.AnonmyMouse)]
    [HttpGet("[action]")]
    [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
    public FileStreamResult Get([FromQuery] string publicId)
    {
        (FileStream fileStream, string contentType) = ViewModel.GetFile(publicId);
        return File(fileStream, contentType);
    }
}
