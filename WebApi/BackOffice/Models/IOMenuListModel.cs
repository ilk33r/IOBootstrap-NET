using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.WebApi.BackOffice.Models
{
    public class IOMenuListModel : IOModel
    {

        #region Properties

        public int ID { get; set; }
        public int MenuOrder { get; set; }
        public int RequiredRole { get; set; }
        public string Action { get; set; }
        public string CssClass { get; set; }
        public string Name { get; set; }
        public IList<IOMenuListModel> ChildItems { get; set; }

        #endregion

        #region Initialization Methods

        public IOMenuListModel() : base()
        {
        }

        #endregion
    }
}
