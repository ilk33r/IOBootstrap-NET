using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers
{
    
    public class IOPushNotificationBackOfficeController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOPushNotificationBackOfficeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {

		#region Initialization Methods

        public IOPushNotificationBackOfficeController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Back Office Methods

        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                // Then return validation error
                return new PushNotificationMessageDeleteResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            ViewModel.DeleteMessage(requestModel.ID);

            // Return response
            return new PushNotificationMessageDeleteResponseModel(IOResponseStatusMessages.OK);
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public ListPushNotificationMessageResponseModel ListMessages()
        {
            // Obtain devices from view model
            IList<PushNotificationMessageModel> messages = ViewModel.ListMessages();

            // Return response
            return new ListPushNotificationMessageResponseModel(IOResponseStatusMessages.OK, messages);
        }

        [IOUserRole(UserRoles.User)]
		[HttpPost]
		public ListPushNotificationResponseModel ListTokens([FromBody] ListPushNotificationRequestModel requestModel)
		{
			// Validate request
			if (!ModelState.IsValid)
			{
				// Then return validation error
                return new ListPushNotificationResponseModel(IOResponseStatusMessages.BAD_REQUEST);
			}

            // Obtain devices from view model
            List<PushNotificationModel> devices = ViewModel.ListTokens(requestModel.Start, requestModel.Limit);

			// Return response
            return new ListPushNotificationResponseModel(IOResponseStatusMessages.OK, devices);
		}

        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
			// Validate request
			if (!ModelState.IsValid)
			{
				// Then return validation error
                return new IOResponseModel(IOResponseStatusMessages.BAD_REQUEST);
			}

            // Obtain device type
            DeviceTypes deviceType = (DeviceTypes)Enum.ToObject(typeof(DeviceTypes), requestModel.DeviceType);

            // Send notification to all devices
            ViewModel.SendNotifications(requestModel.ClientId,
                                         deviceType,
                                         requestModel.NotificationCategory,
                                         requestModel.NotificationData,
                                         requestModel.NotificationMessage,
                                         requestModel.NotificationTitle);

            // Create and return response
            return new IOResponseModel(IOResponseStatusMessages.OK);
        }

		#endregion

	}
	
}
