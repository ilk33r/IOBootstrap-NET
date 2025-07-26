using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Core.Services.Captcha;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.Statics;

public class IOImageAssetViewModel<TDBContext> : IOViewModel<TDBContext>, IIOImageAssetViewModel
    where TDBContext : IODatabaseContext<TDBContext>
{

    private static string CaptchaCacheName = "auth-captcha-{0}";

    public override void CheckAuthorizationHeader()
    {
    }

    public FileStream GetImageFile(string publicId)
    {
        string? fileName = this.GetImageFileName(publicId);
        if (String.IsNullOrEmpty(fileName))
        {
            throw new IOImageNotFoundException();
        }

        string imagesFolder = Configuration.GetValue<string>(IOConfigurationConstants.ImagesFolderKey)!;
        string imagePath = Path.Combine(imagesFolder, fileName);

        if (!File.Exists(imagePath))
        {
            throw new IOImageNotFoundException();
        }

        return File.OpenRead(imagePath);
    }

    public virtual byte[] GetCaptcha(
        string? id,
        IIOCaptchaModule captchaModule
    )
    {
        // Obtain captcha string
        string? captchaString = GetCaptchaString(id);

        // Check captcha is not exists
        if (captchaString == null)
        {
            throw new IOInvalidRequestException();
        }

        // Generate captcha
        return captchaModule.Generate(captchaString, Environment.ContentRootPath);
    }
    
    public static string? GetCaptchaString(string? captchaID)
    {
        // Captcha cache key
        string cacheKey = String.Format(CaptchaCacheName, captchaID);

        // Obtain cached captcha
        IOCacheObject? captchaCache = IOCache.GetCachedObject(cacheKey);
        if (captchaCache == null)
        {
            return null;
        }

        // Obtain captcha string value
        string captchaCacheValue = (string)captchaCache.Value;

        // Check encrypted captcha
        return captchaCacheValue;
    }
}
