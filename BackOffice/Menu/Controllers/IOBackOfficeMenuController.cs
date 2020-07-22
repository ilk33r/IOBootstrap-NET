using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.Menu.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers
{
    public abstract class IOBackOfficeMenuController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOBackOfficeMenuViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        protected IOBackOfficeMenuController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Menu Methods

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuAddResponseModel AddMenuItem([FromBody] IOMenuAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.Name))
            {
                // Then return validation error
                return new IOMenuAddResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Add menu
            ViewModel.AddMenuItem(requestModel);

            // Create and return response
            return new IOMenuAddResponseModel(IOResponseStatusMessages.OK);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuUpdateResponseModel DeleteMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Then return validation error
                return new IOMenuUpdateResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Add menu
            ViewModel.DeleteMenuItem(requestModel.ID);

            // Create and return response
            return new IOMenuUpdateResponseModel(IOResponseStatusMessages.OK);
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public virtual IOMenuListResponseModel ListMenuItems()
        {
            // Obtain menu items
            IList<IOMenuListModel> menuItems = ViewModel.GetMenuTree(ViewModel.UserEntity.UserRole);

            // Create and return response
            return new IOMenuListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), menuItems);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuUpdateResponseModel UpdateMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.Name))
            {
                // Then return validation error
                return new IOMenuUpdateResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Add menu
            ViewModel.UpdateMenuItem(requestModel);

            // Create and return response
            return new IOMenuUpdateResponseModel(IOResponseStatusMessages.OK);
        }

        #endregion

    }
}
