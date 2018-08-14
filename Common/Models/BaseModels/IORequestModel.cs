using IOBootstrap.NET.WebApi.BackOffice.Models;
﻿using IOBootstrap.NET.Common.Enumerations;
using System;

namespace IOBootstrap.NET.Common.Models.BaseModels
{
    public class IORequestModel
    {

        #region Properties

        public IOClientInfoModel ClientInfo { get; set; }
        public CultureTypes Culture;
        public string Version;

        #endregion
    }
}
