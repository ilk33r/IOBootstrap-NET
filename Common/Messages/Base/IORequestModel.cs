using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Messages.Base
{
    public class IORequestModel : IOModel
    {

        #region Properties

        public CultureTypes Culture;
        public string? Version;

        #endregion
    }
}
