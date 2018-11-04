using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOConfigurationUpdateResponseModel : IOConfigurationAddResponseModel
    {
        public IOConfigurationUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }
    }
}
