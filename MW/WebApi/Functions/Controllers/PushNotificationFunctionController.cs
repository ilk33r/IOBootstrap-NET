using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.Functions.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.Functions.Controllers
{
    public class PushNotificationFunctionController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : PushNotificationFunctionViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public PushNotificationFunctionController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<PushNotificationMessageModel> PendingMessages([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<PushNotificationMessageModel> pendingMessages = ViewModel.GetPendingPushNotificationMessages();
            return new IOMWListResponseModel<PushNotificationMessageModel>(pendingMessages);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<PushNotificationDevicesModel> GetDevices([FromBody] IOMWPushNotificationDevicesRequestModel requestModel)
        {
            IList<PushNotificationDevicesModel> devices = ViewModel.GetDevices(requestModel);
            return new IOMWListResponseModel<PushNotificationDevicesModel>(devices);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateDeliveredMessages([FromBody] IOMWUpdatePushNotificationDeliveredMessages requestModel)
        {
            ViewModel.UpdateDeliveredMessages(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel SetMessageSended([FromBody] IOMWFindRequestModel requestModel)
        {
            ViewModel.SetMessageSended(requestModel);
            return new IOResponseModel();
        }
    }
}
