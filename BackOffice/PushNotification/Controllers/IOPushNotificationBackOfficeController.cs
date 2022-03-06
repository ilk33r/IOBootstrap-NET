using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers
{
    [IOBackoffice]
    public class IOPushNotificationBackOfficeController<TViewModel> : IOBackOfficeController<TViewModel> where TViewModel : IOPushNotificationBackOfficeViewModel, new()
    {

		#region Initialization Methods

        public IOPushNotificationBackOfficeController(IConfiguration configuration, 
                                                      IWebHostEnvironment environment, 
                                                      ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region Back Office Methods

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public ListPushNotificationMessageResponseModel ListMessages()
        {
            // Obtain devices from view model
            IList<PushNotificationMessageModel> messages = ViewModel.ListMessages();

            // Return response
            return new ListPushNotificationMessageResponseModel(messages);
        }


        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
            // Send notification to all devices
            ViewModel.SendNotifications(requestModel);

            // Create and return response
            return new IOResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
        {
            ViewModel.DeleteMessage(requestModel.ID);

            // Return response
            return new PushNotificationMessageDeleteResponseModel();
        }

		#endregion

	}
	
}
