using System;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers;

public class IOPushNotificationController<TViewModel, TDBContext, TPushNotificationDevicesEntity> : IOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TViewModel : IOPushNotificationViewModel<TDBContext, TPushNotificationDevicesEntity>, new()
{
    #region Controller Lifecycle

    public IOPushNotificationController(IConfiguration configuration,
                                        IWebHostEnvironment environment,
                                        ILogger<IOLoggerType> logger,
                                        TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Push Notification Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 2)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [HttpPost("[action]")]
    public virtual async Task<AddPushNotificationResponseModel> AddPushNotificationToken([FromBody] AddPushNotificationRequestModel requestModel)
    {
        // Add menu
        await ViewModel.AddPushNotificationToken(requestModel);

        // Create and return response
        return new AddPushNotificationResponseModel();
    }

    #endregion
}
