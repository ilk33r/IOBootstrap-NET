using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOResourceAddRequestModel : IORequestModel
    {
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceAddRequestModel() : base()
        {
        }
    }
}
