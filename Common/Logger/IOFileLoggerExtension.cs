using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Common.Logger
{
    public static class IOFileLoggerExtension
    {
        public static ILoggingBuilder AddIOFileLogger(this ILoggingBuilder builder, Action<IOLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, IOFileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
