using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
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
