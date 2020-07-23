using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOBackOfficeController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> where TViewModel : IOBackOfficeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOBackOfficeController(IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IWebHostEnvironment environment,
                                      ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Update view model request value
            ViewModel.Request = Request;

            if (!ViewModel.IsBackOffice()) 
            {
                // Obtain response model
                IOResponseModel responseModel = new IOResponseModel(IOResponseStatusMessages.INVALID_PERMISSION);

                // Override response
                JsonResult result = new JsonResult(responseModel);
                context.Result = result;

                ActionExecuted = true;
                return;
            }

            base.OnActionExecuting(context);
        }

        #endregion

        #region Client Methods

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.ClientDescription))
            {
                // Create and return response
                return new IOClientAddResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Obtain client info from view model
            IOClientInfoModel clientInfo = ViewModel.CreateClient(requestModel.ClientDescription, requestModel.RequestCount);

            // Create and return response
            return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfo);
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Create and return response
                return new IOResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Check delete client is success
            if (ViewModel.DeleteClient(requestModel.ClientId))
            {
                // Then create and return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return bad request
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpGet]
        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            List<IOClientInfoModel> clientInfos = ViewModel.GetClients();

            // Create and return response
            return new IOClientListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOResponseModel UpdateClient([FromBody] IOClientUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Create and return response
                return new IOResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Check update client is success
            if (ViewModel.UpdateClient(requestModel.ClientId, requestModel.ClientDescription, requestModel.IsEnabled, requestModel.RequestCount, requestModel.MaxRequestCount))
            {
                // Then create and return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return bad request
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        #endregion
    }
}
