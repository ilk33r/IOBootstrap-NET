using System;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
	public class IOMenuUpdateResponseModel : IOMenuAddResponseModel
    {
        public IOMenuUpdateResponseModel(IOResponseStatusModel status) : base(status)
        {
        }
    }
}
