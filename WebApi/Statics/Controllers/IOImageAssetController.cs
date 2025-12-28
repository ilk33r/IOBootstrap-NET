using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Services.Captcha;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.Statics;

public class IOImageAssetController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IOImageAssetViewModel<TDBContext>, new()
{
    public IOImageAssetController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 30)]
    [IOUserRole(UserRoles.AnonmyMouse)]
    [HttpGet("[action]")]
    [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any, NoStore = false)]
    public FileStreamResult Get([FromQuery] string publicId)
    {
        FileStream imageFile = ViewModel.GetImageFile(publicId);
        return File(imageFile, "image/jpeg");
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 30)]
    [IOUserRole(UserRoles.AnonmyMouse)]
    [HttpGet("[action]")]
    public virtual FileContentResult GetCaptcha(
        [FromQuery] string? id,
        [FromServices] IIOCaptchaModule captchaModule
    )
    {
        // Obtain captcha value
        byte[] captcha = ViewModel.GetCaptcha(id, captchaModule);

        // Check if authentication result is true
        return File(captcha, "image/jpeg");
    }
}
