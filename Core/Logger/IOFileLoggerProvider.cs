using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IOBootstrap.NET.Core.Logger
{

    [ProviderAlias("IOFileLogger")]
    public class IOFileLoggerProvider : ILoggerProvider
    {
        public readonly IOLoggerOptions Options;
 
        public IOFileLoggerProvider(IOptions<IOLoggerOptions> options)
        {
            this.Options = options.Value;
 
            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath);
            }
        }
 
        public ILogger CreateLogger(string categoryName)
        {
            return new IOFileLogger(this);
        }
 
        public void Dispose()
        {
        }
    }
}
