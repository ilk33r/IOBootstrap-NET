using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWIntResponseModel : IOResponseModel
    {
        #region Properties

		public int Item { get; set; }

		#endregion

        #region Initialization Methods

		public IOMWIntResponseModel() : base() 
		{
		}

		public IOMWIntResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

        public IOMWIntResponseModel(int responseStatusMessage, int item) : base(responseStatusMessage)
		{
            Item = item;
		}

		#endregion
    }

}
