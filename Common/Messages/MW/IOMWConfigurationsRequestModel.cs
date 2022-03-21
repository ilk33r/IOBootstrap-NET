using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWConfigurationsRequestModel : IORequestModel
    {
        public IList<string> ConfigKeys { get; set; }
    }
}
