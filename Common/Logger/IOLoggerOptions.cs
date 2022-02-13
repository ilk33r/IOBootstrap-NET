using System;

namespace IOBootstrap.NET.Common.Logger
{
    public class IOLoggerOptions
    {
        public virtual bool Enabled { get; set; }
        
        public virtual string FilePath { get; set; }
 
        public virtual string FolderPath { get; set; }
    }
}
