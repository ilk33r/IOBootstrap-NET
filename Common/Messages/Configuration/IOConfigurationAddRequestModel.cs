using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
    public class IOConfigurationAddRequestModel : IORequestModel
    {

        public string ConfigKey { get; set; }
        public string StrValue { get; set; }
        public int IntValue { get; set; }

        public IOConfigurationAddRequestModel() : base()
        {
        }
    }
}
