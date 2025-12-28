using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers;

[IOBackoffice]
public class IOPushNotificationBackOfficeController<TViewModel, TDBContext, TPushNotificationDevicesEntity> : IOBackOfficeController<TViewModel, TDBContext>
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
where TViewModel : IOPushNotificationBackOfficeViewModel<TDBContext, TPushNotificationDevicesEntity>, new()
{

    #region Initialization Methods

    public IOPushNotificationBackOfficeController(IConfiguration configuration,
                                                  IWebHostEnvironment environment,
                                                  ILogger<IOLoggerType> logger,
                                                  TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Back Office Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.User)]
    [HttpPost("[action]")]
    public virtual ListPushNotificationMessageResponseModel ListMessages([FromBody] IOListPushNotificationsRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckPushNotificationsIsEnabled();

        // Return response
        return ViewModel.ListMessages(requestModel);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 5)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.User)]
    [HttpPost("[action]")]
    public virtual IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckPushNotificationsIsEnabled();

        // Send notification to all devices
        ViewModel.SendNotifications(requestModel);

        // Create and return response
        return new IOResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.User)]
    [HttpPost("[action]")]
    public virtual PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckPushNotificationsIsEnabled();

        // Delete message
        ViewModel.DeleteMessage(requestModel.ID);

        // Return response
        return new PushNotificationMessageDeleteResponseModel();
    }

    #endregion

}
