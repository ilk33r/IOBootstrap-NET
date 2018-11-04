using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
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
