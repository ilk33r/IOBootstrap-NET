using System;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.Configuration
{
    public class IOConfigurationModel : IOModel
    {
        public int ID { get; set; }
        public string ConfigKey { get; set; }
        public int? ConfigIntValue { get; set; }
        public string ConfigStringValue { get; set; }
    }
}
