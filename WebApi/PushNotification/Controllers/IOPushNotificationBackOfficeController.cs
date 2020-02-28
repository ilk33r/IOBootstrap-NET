using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
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

namespace IOBootstrap.WebApi.PushNotification.Controllers
{
    
    [Route("backoffice/[controller]")]
    public class IOPushNotificationBackOfficeController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOPushNotificationBackOfficeViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

		#region Initialization Methods

		public IOPushNotificationBackOfficeController(ILoggerFactory factory, 
                                                    ILogger<TLogger> logger, 
                                                    IConfiguration configuration, 
                                                    TDBContext databaseContext,
                                                    IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
		}

        #endregion

        #region Back Office Methods

        [IOUserRole(UserRoles.User)]
        [HttpPost("[action]")]
        public PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new PushNotificationMessageDeleteResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            _viewModel.DeleteMessage(requestModel.ID);

            // Return response
            return new PushNotificationMessageDeleteResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.User)]
        [HttpGet("[action]")]
        public ListPushNotificationMessageResponseModel ListMessages()
        {
            // Obtain devices from view model
            IList<PushNotificationMessageModel> messages = _viewModel.ListMessages();

            // Return response
            return new ListPushNotificationMessageResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), messages);
        }

        [IOUserRole(UserRoles.User)]
		[HttpPost("[action]")]
		public ListPushNotificationResponseModel ListTokens([FromBody] ListPushNotificationRequestModel requestModel)
		{
			// Validate request
			if (requestModel == null)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new ListPushNotificationResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), null);
			}

            // Obtain devices from view model
            List<PushNotificationModel> devices = _viewModel.ListTokens(requestModel.Start, requestModel.Limit);

			// Return response
            return new ListPushNotificationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), devices);
		}

        [IOUserRole(UserRoles.User)]
        [HttpPost("[action]")]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.NotificationMessage))
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new IOResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
			}

            // Obtain device type
            DeviceTypes deviceType = (DeviceTypes)Enum.ToObject(typeof(DeviceTypes), requestModel.DeviceType);

            // Send notification to all devices
            _viewModel.SendNotifications(requestModel.ClientId,
                                         deviceType,
                                         requestModel.NotificationCategory,
                                         requestModel.NotificationData,
                                         requestModel.NotificationMessage,
                                         requestModel.NotificationTitle);

            // Create and return response
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

		#endregion

	}
	
}
