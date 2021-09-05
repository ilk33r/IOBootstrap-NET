using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Messages.Controllers
{
    [IOBackoffice]
    public abstract class IOBackOfficeMessagesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOBackOfficeMessagesViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        
        #region Controller Lifecycle

        protected IOBackOfficeMessagesController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
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
