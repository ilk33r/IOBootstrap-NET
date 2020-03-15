using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOResourceUpdateRequestModel : IORequestModel
    {
        public int ResourceID { get; set; }
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }

        public IOResourceUpdateRequestModel() : base()
        {
        }
    }
}