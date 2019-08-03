using System;
using System.Collections.Generic;
using IOBootstrap.NET.Application;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Configuration;
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
    public class IOBackOfficeConfigurationsController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOBackOfficeConfigurationsViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOBackOfficeConfigurationsController(ILoggerFactory factory,
                                      ILogger<TLogger> logger,
                                      IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region Configuration Methods

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationAddResponseModel AddConfigItem([FromBody] IOConfigurationAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.ConfigKey))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOConfigurationAddResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.AddConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationDeleteResponseModel DeleteConfigItem([FromBody] IOConfigurationDeleteRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOConfigurationDeleteResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.DeleteConfigItem(requestModel.ConfigId);

            // Create and return response
            return new IOConfigurationDeleteResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public IOConfigurationListResponseModel ListConfigurationItems()
        {
            // Obtain configuration items
            IList<IOConfigurationEntity> configurationItems = _viewModel.GetConfigurations();

            // Create and return response
            return new IOConfigurationListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), configurationItems);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationUpdateResponseModel UpdateConfigItem([FromBody] IOConfigurationUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.ConfigKey))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOConfigurationUpdateResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.UpdateConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationUpdateResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public virtual IOConfigurationUpdateResponseModel RestartApp()
        {
            // Create and return response
            // Program.Shutdown();
            return new IOConfigurationUpdateResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        #endregion
    }
}
