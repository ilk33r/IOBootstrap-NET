using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Common.Logger
{
    public class IOFileLogger: ILogger
    {
        protected readonly IOFileLoggerProvider FileLoggerProvider;
 
        public IOFileLogger(IOFileLoggerProvider fileLoggerProvider)
        {
            this.FileLoggerProvider = fileLoggerProvider;
        }
 
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
 
        public bool IsEnabled(LogLevel logLevel)
        {
            return FileLoggerProvider.Options.Enabled;
        }
 
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
 
            string fullFilePath = FileLoggerProvider.Options.FolderPath + "/" + FileLoggerProvider.Options.FilePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
            string logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");
 
            try {
                using (var streamWriter = new StreamWriter(fullFilePath, true))
                {
                    streamWriter.WriteLine(logRecord);
                }
            } 
            catch (Exception) 
            {
            }
        }
    }
}
