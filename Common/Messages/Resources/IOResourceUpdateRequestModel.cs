using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Resources
{
    public class IOResourceUpdateRequestModel : IOResourceAddRequestModel
    {
        [Required]
        public int ResourceID { get; set; }

        public IOResourceUpdateRequestModel() : base()
        {
        }
    }
}