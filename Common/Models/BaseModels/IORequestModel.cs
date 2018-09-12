using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Shared;
using System;

namespace IOBootstrap.NET.Common.Models.BaseModels
{
    public class IORequestModel : IOModel
    {

        #region Properties

        public CultureTypes Culture;
        public string Version;

        public IORequestModel() : base()
        {
        }

        #endregion
    }
}
