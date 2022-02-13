using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        //TODO: Migate with MW.
        /*
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
        {
            ViewModel.DeleteMessage(requestModel.ID);

            // Return response
            return new PushNotificationMessageDeleteResponseModel();
        }

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
		public ListPushNotificationResponseModel ListTokens([FromBody] ListPushNotificationRequestModel requestModel)
		{
            // Obtain devices from view model
            List<PushNotificationModel> devices = ViewModel.ListTokens(requestModel.Start, requestModel.Limit);

			// Return response
            return new ListPushNotificationResponseModel(devices);
		}

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
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
            return new IOResponseModel();
        }
        */
		#endregion

	}
	
}
