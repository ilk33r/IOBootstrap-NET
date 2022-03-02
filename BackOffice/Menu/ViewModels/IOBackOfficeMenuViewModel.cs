using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Menu.ViewModels
{
    public class IOBackOfficeMenuViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOBackOfficeMenuViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

        public void AddMenuItem(IOMenuAddRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            MWConnector.Get<IOResponseModel>(controller + "/" + "AddMenuItem", requestModel);
        }

        public void DeleteMenuItem(int menuId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = menuId
            };
            MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteMenuItem", requestModel);
        }

        public virtual IList<IOMenuListModel> GetMenuTree(int requiredRole)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = requiredRole
            };
            IOMWListResponseModel<IOMenuListModel> menuTreeResponse = MWConnector.Get<IOMWListResponseModel<IOMenuListModel>>(controller + "/" + "GetMenuTree", requestModel);
            return menuTreeResponse.Items;
        }

        public void UpdateMenuItem(IOMenuUpdateRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateMenuItem", requestModel);
        }

        #endregion
    }
}
