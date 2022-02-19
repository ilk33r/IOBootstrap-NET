using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.Net.Common.Messages.MW
{
    public class IOMWListResponseModel<TObject> : IOResponseModel where TObject : IOModel
    {
        #region Properties

		public IList<TObject> Items { get; set; }

		#endregion

        #region Initialization Methods

		public IOMWListResponseModel() : base() 
		{
		}

		public IOMWListResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

        public IOMWListResponseModel(IList<TObject> items) : base() 
		{
            Items = items;
		}

		#endregion
    }
}
