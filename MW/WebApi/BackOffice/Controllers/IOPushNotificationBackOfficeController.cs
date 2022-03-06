using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOPushNotificationBackOfficeController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOPushNotificationBackOfficeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOPushNotificationBackOfficeController(IConfiguration configuration, 
                                                      TDBContext databaseContext, 
                                                      IWebHostEnvironment environment, 
                                                      ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<PushNotificationMessageModel> ListMessages([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<PushNotificationMessageModel> messages = ViewModel.ListMessages();
            return new IOMWListResponseModel<PushNotificationMessageModel>(messages);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
            ViewModel.SendNotifications(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel DeleteMessage([FromBody] IOMWFindRequestModel requestModel)
        {
            int status = ViewModel.DeleteMessage(requestModel);
            return new IOResponseModel(status);
        }
    }
}
