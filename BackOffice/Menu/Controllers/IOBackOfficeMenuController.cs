using System;
using IOBootstrap.NET.BackOffice.Menu.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers;

[IOBackoffice]
public abstract class IOBackOfficeMenuController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOBackOfficeMenuViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    protected IOBackOfficeMenuController(IConfiguration configuration,
                                         IWebHostEnvironment environment,
                                         ILogger<IOLoggerType> logger,
                                         TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Menu Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMenuAddResponseModel AddMenuItem([FromBody] IOMenuAddRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckMenuIsEnabled();

        // Add menu
        ViewModel.AddMenuItem(requestModel);

        // Create and return response
        return new IOMenuAddResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMenuUpdateResponseModel DeleteMenuItem([FromBody] IOMenuDeleteRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckMenuIsEnabled();

        // Add menu
        ViewModel.DeleteMenuItem(requestModel.ID);

        // Create and return response
        return new IOMenuUpdateResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpGet("[action]")]
    public virtual IOMenuListResponseModel ListMenuItems()
    {
        // Obtain menu items
        IList<IOMenuListModel> menuItems = ViewModel.GetMenuTree(ViewModel.UserModel?.UserRole ?? (int)UserRoles.AnonmyMouse);

        // Create and return response
        return new IOMenuListResponseModel(menuItems);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMenuUpdateResponseModel UpdateMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckMenuIsEnabled();
        
        // Add menu
        ViewModel.UpdateMenuItem(requestModel);

        // Create and return response
        return new IOMenuUpdateResponseModel();
    }

    #endregion

}
