using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOPushNotificationViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOPushNotificationController(IConfiguration configuration, 
                                            TDBContext databaseContext, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel AddPushNotificationTokenV2([FromBody] AddPushNotificationRequestModel requestModel)
        {
            ViewModel.AddTokenV2(requestModel);
            return new IOResponseModel();
        }
    }
}
