using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Menu
{
	public class IOMenuListResponseModel : IOResponseModel
    {
        #region Properties

		public IList<IOMenuListModel> items { get; set; }

        #endregion

        #region Initialization Methods

		public IOMenuListResponseModel(IOResponseStatusModel status) : base(status)
        {
        }

		public IOMenuListResponseModel(IOResponseStatusModel status, IList<IOMenuListModel> items) : base(status)
        {
            this.items = items;
        }

        #endregion

    }
}
