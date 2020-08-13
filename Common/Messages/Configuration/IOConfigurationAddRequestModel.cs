using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.Configuration
{
    public class IOConfigurationAddRequestModel : IORequestModel
    {

        [Required]
        public string ConfigKey { get; set; }
        public string StrValue { get; set; }
        public int? IntValue { get; set; }

        public IOConfigurationAddRequestModel() : base()
        {
        }
    }
}
