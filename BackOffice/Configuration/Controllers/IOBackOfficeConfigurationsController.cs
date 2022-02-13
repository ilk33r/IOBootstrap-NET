using System;
using IOBootstrap.NET.BackOffice.Configuration.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeConfigurationsController<TViewModel> : IOBackOfficeController<TViewModel> where TViewModel : IOBackOfficeConfigurationsViewModel, new()
    {
        
        #region Controller Lifecycle

        public IOBackOfficeConfigurationsController(IConfiguration configuration, 
                                                    IWebHostEnvironment environment, 
                                                    ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region Configuration Methods

        //TODO: Migrate with MW:
        /*
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationAddResponseModel AddConfigItem([FromBody] IOConfigurationAddRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationAddResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationDeleteResponseModel DeleteConfigItem([FromBody] IOConfigurationDeleteRequestModel requestModel)
        {
            // Add menu
            ViewModel.DeleteConfigItem(requestModel.ConfigId);

            // Create and return response
            return new IOConfigurationDeleteResponseModel();
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public IOConfigurationListResponseModel ListConfigurationItems()
        {
            // Obtain configuration items
            IList<IOConfigurationModel> configurationItems = ViewModel.GetConfigurations();

            // Create and return response
            return new IOConfigurationListResponseModel(configurationItems);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOConfigurationUpdateResponseModel UpdateConfigItem([FromBody] IOConfigurationUpdateRequestModel requestModel)
        {
            // Add menu
            ViewModel.UpdateConfigItem(requestModel);

            // Create and return response
            return new IOConfigurationUpdateResponseModel();
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public virtual IOConfigurationUpdateResponseModel ResetCache()
        {
            IOCache.ClearCache();
            return new IOConfigurationUpdateResponseModel();
        }
        */
        #endregion
    }
}
