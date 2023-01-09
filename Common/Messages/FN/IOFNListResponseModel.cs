using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Messages.FN
{
    public class IOFNListResponseModel<TObject> : IOResponseModel where TObject : IOModel
    {
        #region Properties

		public int? Count { get; set; }
		public IList<TObject> Items { get; set; }

		#endregion

        #region Initialization Methods

		public IOFNListResponseModel() : base() 
		{
		}

		public IOFNListResponseModel(int responseStatusMessage) : base(responseStatusMessage) 
		{
		}

        public IOFNListResponseModel(IList<TObject> items) : base() 
		{
            Items = items;
		}

		#endregion
    }
}
