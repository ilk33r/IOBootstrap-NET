using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeMenuController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOBackOfficeMenuViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOBackOfficeMenuController(ILoggerFactory factory,
                                      ILogger<TLogger> logger,
                                      IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region Menu Methods

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public IOMenuListResponseModel ListMenuItems()
        {
            // Obtain menu items
            IList<IOMenuListModel> menuItems = _viewModel.GetMenuTree(_viewModel.userEntity.UserRole);

            // Create and return response
            return new IOMenuListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), menuItems);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuAddResponseModel AddMenuItem([FromBody] IOMenuAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.Name))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOMenuAddResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.AddMenuItem(requestModel);

            // Create and return response
            return new IOMenuAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMenuUpdateResponseModel UpdateMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.Name))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOMenuUpdateResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.UpdateMenuItem(requestModel);

            // Create and return response
            return new IOMenuUpdateResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        #endregion

    }
}
