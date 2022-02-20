using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.Net.Common.Messages.MW
{
    public class IOMWObjectResponseModel<TObject> : IOResponseModel where TObject : IOModel
    {
        #region Properties

		public TObject Item { get; set; }

		#endregion

        #region Initialization Methods

		public IOMWObjectResponseModel() : base() 
		{
		}

		public IOMWObjectResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

        public IOMWObjectResponseModel(TObject item) : base() 
		{
            Item = item;
		}

		#endregion
    }
}
