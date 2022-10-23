using System;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.Menu.Interfaces;

namespace IOBootstrap.NET.BackOffice.Menu.ViewModels
{
    public class IOBackOfficeMenuViewModel : IOBackOfficeViewModel, IIOBackOfficeMenuViewModel
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
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "AddMenuItem", requestModel);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }

        public void DeleteMenuItem(int menuId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = menuId
            };
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteMenuItem", requestModel);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }

        public IList<IOMenuListModel> GetMenuTree(int requiredRole)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = requiredRole
            };
            IOMWListResponseModel<IOMenuListModel> menuTreeResponse = MWConnector.Get<IOMWListResponseModel<IOMenuListModel>>(controller + "/" + "GetMenuTree", requestModel);
            if (MWConnector.HandleResponse(menuTreeResponse, code => {}))
            {
                return menuTreeResponse.Items;
            }

            return new List<IOMenuListModel>();
        }

        public void UpdateMenuItem(IOMenuUpdateRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateMenuItem", requestModel);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }

        #endregion
    }
}
