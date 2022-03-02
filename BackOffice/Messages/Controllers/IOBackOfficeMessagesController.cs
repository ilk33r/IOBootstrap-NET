using System;
using IOBootstrap.NET.BackOffice.Messages.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Messages.Controllers
{
    [IOBackoffice]
    public abstract class IOBackOfficeMessagesController<TViewModel> : IOBackOfficeController<TViewModel> where TViewModel : IOBackOfficeMessagesViewModel, new()
    {
        
        #region Controller Lifecycle

        protected IOBackOfficeMessagesController(IConfiguration configuration, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public virtual IOListMessagesResponseModel ListMessages()
        {
            // Obtain message items
            IList<IOMessageModel> messages = ViewModel.GetMessages();

            // Create and return response
            return new IOListMessagesResponseModel(messages);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public IOListMessagesResponseModel ListAllMessages()
        {
            // Obtain message items
            IList<IOMessageModel> messages = ViewModel.GetAllMessages();

            // Create and return response
            return new IOListMessagesResponseModel(messages);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMessageAddResponseModel AddMessagesItem([FromBody] IOMessageAddRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddMessage(requestModel);

            // Create and return response
            return new IOMessageAddResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMessageDeleteResponseModel DeleteMessagesItem([FromBody] IOMessageDeleteRequestModel requestModel) 
        {
            // Add menu
            ViewModel.DeleteMessage(requestModel.MessageId);

            // Create and return response
            return new IOMessageDeleteResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMessageUpdateResponseModel UpdateMessagesItem([FromBody] IOMessageUpdateRequestModel requestModel)
        {
            // Add menu
            ViewModel.UpdateMessage(requestModel);

            // Create and return response
            return new IOMessageUpdateResponseModel();
        }
    }
}
