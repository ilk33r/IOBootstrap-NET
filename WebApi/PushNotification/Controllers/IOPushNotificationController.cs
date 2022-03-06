using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationController<TViewModel> : IOController<TViewModel> where TViewModel : IOPushNotificationViewModel, new()
    {
        #region Controller Lifecycle

        public IOPushNotificationController(IConfiguration configuration, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
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
