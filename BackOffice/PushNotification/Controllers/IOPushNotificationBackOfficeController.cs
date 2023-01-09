using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers
{
    [IOBackoffice]
    public class IOPushNotificationBackOfficeController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> 
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IOPushNotificationBackOfficeViewModel<TDBContext>, new()
    {

		#region Initialization Methods

        public IOPushNotificationBackOfficeController(IConfiguration configuration, 
                                                      IWebHostEnvironment environment, 
                                                      ILogger<IOLoggerType> logger,
                                                      TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }

        #endregion

        #region Back Office Methods

        [IOUserRole(UserRoles.User)]
        [HttpGet("[action]")]
        public ListPushNotificationMessageResponseModel ListMessages()
        {
            // Obtain devices from view model
            IList<PushNotificationMessageModel> messages = ViewModel.ListMessages();

            // Return response
            return new ListPushNotificationMessageResponseModel(messages);
        }


        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost("[action]")]
        public IOResponseModel SendNotification([FromBody] SendPushNotificationRequestModel requestModel)
        {
            // Send notification to all devices
            ViewModel.SendNotifications(requestModel);

            // Create and return response
            return new IOResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
        [HttpPost("[action]")]
        public PushNotificationMessageDeleteResponseModel DeleteMessage([FromBody] PushNotificationMessageDeleteRequestModel requestModel)
        {
            ViewModel.DeleteMessage(requestModel.ID);

            // Return response
            return new PushNotificationMessageDeleteResponseModel();
        }

		#endregion

	}
	
}
