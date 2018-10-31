using System;
using IOBootstrap.NET.Common.Models.BaseModels;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMessageDeleteRequestModel : IORequestModel
    {

        public int MessageId { get; set; }

        public IOMessageDeleteRequestModel() : base()
        {
        }
    }
}
