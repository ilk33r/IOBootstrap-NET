using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWStringResponseModel : IOResponseModel
    {
        #region Properties

		public string Item { get; set; }

		#endregion

        #region Initialization Methods

		public IOMWStringResponseModel() : base() 
		{
		}

		public IOMWStringResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

        public IOMWStringResponseModel(int responseStatusMessage, string item) : base(responseStatusMessage)
		{
            Item = item;
		}

		#endregion
    }
}
