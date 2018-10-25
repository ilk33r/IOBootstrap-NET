using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMenuAddRequestModel : IORequestModel
    {

        public string Action { get; set; }
        public string CssClass { get; set; }
        public string Name { get; set; }
        public int MenuOrder { get; set; }
        public int RequiredRole { get; set; }
        public Nullable<int> ParentEntityID { get; set; }

        public IOMenuAddRequestModel() : base()
        {
        }
    }
}
