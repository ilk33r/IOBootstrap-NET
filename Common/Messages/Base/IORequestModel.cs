using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Base
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
