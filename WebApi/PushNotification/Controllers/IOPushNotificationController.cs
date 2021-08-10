using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> where TViewModel : IOPushNotificationViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOPushNotificationController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Push Notification Methods

        [IOValidateRequestModel]
        [HttpPost]
        public virtual AddPushNotificationResponseModel AddPushNotificationToken([FromBody] AddPushNotificationRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddToken(requestModel);

            // Create and return response
            return new AddPushNotificationResponseModel(IOResponseStatusMessages.OK);
        }

        #endregion
    }
}
