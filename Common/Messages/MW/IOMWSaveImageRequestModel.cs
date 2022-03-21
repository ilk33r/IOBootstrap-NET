using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWSaveImageRequestModel : IORequestModel
    {
        public IList<IOImageVariationsModel> Variations { get; set; }
    }
}
