using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOResourceModel : IOModel
    {

        public int ResourceID { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceModel() : base()
        {
        }
    }
}
