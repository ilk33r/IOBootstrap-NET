using System;

namespace IOBootstrap.NET.Common.Models.Shared
{
    public class IOClientInfoModel
    {

        #region Properties

        public String ClientID { get; set; }
        public String ClientSecret { get; set; }

        #endregion

        #region Initialization Methods

        public IOClientInfoModel(String clientId, String clientSectet)
        {
            // Setup properties
            this.ClientID = clientId;
            this.ClientSecret = clientSectet;
        }

        #endregion
    }
}
