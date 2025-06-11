using System;
using IOBootstrap.NET.BackOffice.Configuration.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers;

[IOBackoffice]
public class IOBackOfficeConfigurationsController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOBackOfficeConfigurationsViewModel<TDBContext>, new()
{

    #region Controller Lifecycle

    public IOBackOfficeConfigurationsController(IConfiguration configuration,
                                                IWebHostEnvironment environment,
                                                ILogger<IOLoggerType> logger,
                                                TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Configuration Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOConfigurationAddResponseModel AddConfigItem([FromBody] IOConfigurationAddRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckConfigurationsIsEnabled();

        // Add menu
        ViewModel.AddConfigItem(requestModel);

        // Create and return response
        return new IOConfigurationAddResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOConfigurationDeleteResponseModel DeleteConfigItem([FromBody] IOConfigurationDeleteRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckConfigurationsIsEnabled();

        // Add menu
        ViewModel.DeleteConfigItem(requestModel.ConfigId);

        // Create and return response
        return new IOConfigurationDeleteResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.User)]
    [HttpGet("[action]")]
    public IOConfigurationListResponseModel ListConfigurationItems()
    {
        // Check enabled
        ViewModel.CheckConfigurationsIsEnabled();

        // Obtain configuration items
        IList<IOConfigurationModel> configurationItems = ViewModel.GetConfigurations();

        // Create and return response
        return new IOConfigurationListResponseModel(configurationItems);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOConfigurationUpdateResponseModel UpdateConfigItem([FromBody] IOConfigurationUpdateRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckConfigurationsIsEnabled();

        // Add menu
        ViewModel.UpdateConfigItem(requestModel);

        // Create and return response
        return new IOConfigurationUpdateResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpGet("[action]")]
    public virtual IOConfigurationUpdateResponseModel ResetCache()
    {
        // Check enabled
        ViewModel.CheckConfigurationsIsEnabled();
        
        IOCache.ClearCache();
        return new IOConfigurationUpdateResponseModel();
    }

    #endregion
}
