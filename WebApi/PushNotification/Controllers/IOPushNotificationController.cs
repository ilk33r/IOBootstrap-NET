using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> 
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IOPushNotificationViewModel<TDBContext>, new()
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

        [IOValidateRequestModel]
        [HttpPost]
        public virtual AddPushNotificationResponseModel AddPushNotificationTokenV2([FromBody] AddPushNotificationRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddTokenV2(requestModel);

            // Create and return response
            return new AddPushNotificationResponseModel();
        }

        #endregion
    }
}
