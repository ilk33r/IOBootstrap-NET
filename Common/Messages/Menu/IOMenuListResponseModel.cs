using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Menu;

namespace IOBootstrap.NET.Common.Messages.Menu
{
	public class IOMenuListResponseModel : IOResponseModel
    {
        #region Properties

		public IList<IOMenuListModel> items { get; set; }

        #endregion

        #region Initialization Methods

		public IOMenuListResponseModel(IList<IOMenuListModel> items) : base()
        {
            this.items = items;
        }

        #endregion

    }
}
