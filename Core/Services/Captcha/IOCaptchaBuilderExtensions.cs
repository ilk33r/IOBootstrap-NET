using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IOBootstrap.NET.Core.Services.Captcha;

public static class IOCaptchaBuilderExtensions
{
    public static IServiceCollection AddCaptcha(
        this IServiceCollection services,
        Action<IOCaptchaOptions> setupAction)
    {
        var options = new IOCaptchaOptions();
        setupAction?.Invoke(options);

        services.TryAddSingleton<IIOCaptchaModule>(new IOCaptchaModule(options));
        return services;
    }
}
