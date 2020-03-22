using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOResourceDeleteRequestModel : IORequestModel
    {
        public int ID { get; set; }

        public IOResourceDeleteRequestModel() : base()
        {
        }
    }
}