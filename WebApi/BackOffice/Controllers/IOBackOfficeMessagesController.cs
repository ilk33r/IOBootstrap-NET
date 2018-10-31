using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeMessagesController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOBackOfficeMessagesViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOBackOfficeMessagesController(ILoggerFactory factory,
                                      ILogger<TLogger> logger,
                                      IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        [IOUserRole(UserRoles.User)]
        [HttpGet]
        public IOListMessagesResponseModel ListMessages()
        {
            // Obtain message items
            IList<IOMessageModel> messages = _viewModel.GetMessages();

            // Create and return response
            return new IOListMessagesResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), messages);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public IOListMessagesResponseModel ListAllMessages()
        {
            // Obtain message items
            IList<IOMessageModel> messages = _viewModel.GetAllMessages();

            // Create and return response
            return new IOListMessagesResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), messages);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMessageAddResponseModel AddMessagesItem([FromBody] IOMessageAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.Message))
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOMessageAddResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.AddMessage(requestModel);

            // Create and return response
            return new IOMessageAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOMessageDeleteResponseModel DeleteMessagesItem([FromBody] IOMessageDeleteRequestModel requestModel) 
        {
            // Validate request
            if (requestModel == null)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOMessageDeleteResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.DeleteMessage(requestModel.MessageId);

            // Create and return response
            return new IOMessageDeleteResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }
    }
}
