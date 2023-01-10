using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.FN;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.FunctionsApi.PushNotificationFunction.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.FunctionsApi.PushNotificationFunction.Controllers
{
    public class PushNotificationFunctionController<TViewModel, TDBContext> : IOFunctionController<TViewModel, TDBContext>
    where TViewModel : PushNotificationFunctionViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public PushNotificationFunctionController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }

        [IORequireHTTPS]
        [HttpPost("[action]")]
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.AnonmyMouse)]
        public IOFNListResponseModel<PushNotificationMessageModel> PendingMessages([FromBody] IOFNFindRequestModel requestModel)
        {
            IList<PushNotificationMessageModel> response = ViewModel.GetPendingPushNotificationMessages();
            return new IOFNListResponseModel<PushNotificationMessageModel>(response);
        }

        [IORequireHTTPS]
        [HttpPost("[action]")]
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.AnonmyMouse)]
        public IOFNListResponseModel<PushNotificationDevicesModel> GetDevices([FromBody] IOFNPushNotificationDevicesRequestModel requestModel)
        {
            IList<PushNotificationDevicesModel> response = ViewModel.GetDevices(requestModel);
            return new IOFNListResponseModel<PushNotificationDevicesModel>(response);
        }

        [IORequireHTTPS]
        [HttpPost("[action]")]
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.AnonmyMouse)]
        public IOResponseModel UpdateDeliveredMessages([FromBody] IOFNUpdatePushNotificationDeliveredMessages requestModel)
        {
            ViewModel.UpdateDeliveredMessages(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost("[action]")]
        [IOUserRole(UserRoles.AnonmyMouse)]
        public IOResponseModel SetMessageSended([FromBody] IOFNFindRequestModel requestModel)
        {
            ViewModel.SetMessageSended(requestModel);
            return new IOResponseModel();
        }
    }
}
