using System;
using IOBootstrap.NET.BackOffice.Menu.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers
{
    [IOBackoffice]
    public abstract class IOBackOfficeMenuController<TViewModel> : IOBackOfficeController<TViewModel> where TViewModel : IOBackOfficeMenuViewModel, new()
    {
        #region Controller Lifecycle

        protected IOBackOfficeMenuController(IConfiguration configuration, 
                                             IWebHostEnvironment environment, 
                                             ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region Menu Methods

        //TODO: Migrate with MW.
        /*
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuAddResponseModel AddMenuItem([FromBody] IOMenuAddRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddMenuItem(requestModel);

            // Create and return response
            return new IOMenuAddResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuUpdateResponseModel DeleteMenuItem([FromBody] IOMenuDeleteRequestModel requestModel)
        {
            // Add menu
            ViewModel.DeleteMenuItem(requestModel.ID);

            // Create and return response
            return new IOMenuUpdateResponseModel();
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public virtual IOMenuListResponseModel ListMenuItems()
        {
            // Obtain menu items
            IList<IOMenuListModel> menuItems = ViewModel.GetMenuTree(ViewModel.UserEntity.UserRole);

            // Create and return response
            return new IOMenuListResponseModel(menuItems);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuUpdateResponseModel UpdateMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
        {
            // Add menu
            ViewModel.UpdateMenuItem(requestModel);

            // Create and return response
            return new IOMenuUpdateResponseModel();
        }
        */
        #endregion

    }
}
