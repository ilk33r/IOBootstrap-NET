using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.Configuration.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers
{
    public class IOBackOfficeConfigurationsController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOBackOfficeConfigurationsViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        
        #region Controller Lifecycle

        public IOBackOfficeConfigurationsController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Configuration Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationAddResponseModel AddConfigItem([FromBody] IOConfigurationAddRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationAddResponseModel(IOResponseStatusMessages.OK);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationDeleteResponseModel DeleteConfigItem([FromBody] IOConfigurationDeleteRequestModel requestModel)
        {
            // Add menu
            ViewModel.DeleteConfigItem(requestModel.ConfigId);

            // Create and return response
            return new IOConfigurationDeleteResponseModel(IOResponseStatusMessages.OK);
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public IOConfigurationListResponseModel ListConfigurationItems()
        {
            // Obtain configuration items
            IList<IOConfigurationModel> configurationItems = ViewModel.GetConfigurations();

            // Create and return response
            return new IOConfigurationListResponseModel(IOResponseStatusMessages.OK, configurationItems);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationUpdateResponseModel UpdateConfigItem([FromBody] IOConfigurationUpdateRequestModel requestModel)
        {
            // Add menu
            ViewModel.UpdateConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationUpdateResponseModel(IOResponseStatusMessages.OK);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public virtual IOConfigurationUpdateResponseModel ResetCache()
        {
            IOCache.ClearCache();
            return new IOConfigurationUpdateResponseModel(IOResponseStatusMessages.OK);
        }

        #endregion
    }
}
