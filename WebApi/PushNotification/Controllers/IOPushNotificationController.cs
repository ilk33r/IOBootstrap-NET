using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.PushNotification.Models;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace BoomApp.WebApi.PushNotification.Controllers
{

    [Route("api/[controller]")]
    public class IOPushNotificationsController<TLogger, TViewModel, TDBContext> : IOController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOPushNotificationViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOPushNotificationsController(ILoggerFactory factory,
                                           ILogger<TLogger> logger,
                                           IConfiguration configuration,
                                           TDBContext databaseContext,
                                           IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region Api Methods

        [HttpPost("[action]")]
        public AddPushNotificationResponseModel AddToken([FromBody] AddPushNotificationRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.AppBundleId)
                || String.IsNullOrEmpty(requestModel.AppVersion)
                || String.IsNullOrEmpty(requestModel.DeviceId)
                || String.IsNullOrEmpty(requestModel.DeviceName)
                || String.IsNullOrEmpty(requestModel.DeviceToken))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new AddPushNotificationResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add token to database
            _viewModel.AddToken(requestModel.AppBuildNumber, requestModel.AppBundleId,
                                requestModel.AppVersion,
                                requestModel.DeviceId,
                                requestModel.DeviceName,
                                requestModel.DeviceToken,
                                requestModel.DeviceType);

            // Create and return response
            return new AddPushNotificationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        #endregion

    }
}
