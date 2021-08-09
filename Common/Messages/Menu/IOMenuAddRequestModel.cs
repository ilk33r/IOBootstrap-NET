using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Menu
{
    public class IOMenuAddRequestModel : IORequestModel
    {

        public string Action { get; set; }
        
        public string CssClass { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int MenuOrder { get; set; }

        [Required]
        public int RequiredRole { get; set; }

        public Nullable<int> ParentEntityID { get; set; }

        public IOMenuAddRequestModel() : base()
        {
        }
    }
}
