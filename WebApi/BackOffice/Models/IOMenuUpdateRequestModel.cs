using System;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMenuUpdateRequestModel : IOMenuAddRequestModel
    {
        public int ID { get; set; }

        public IOMenuUpdateRequestModel() : base()
        {
        }
    }
}
