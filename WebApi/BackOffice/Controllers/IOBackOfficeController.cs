using IOBootstrap.NET.Common.Constants;
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
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeController<TLogger, TViewModel, TDBContext>: IOController<TLogger, TViewModel, TDBContext>
        where TViewModel: IOBackOfficeViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOBackOfficeController(ILoggerFactory factory, 
                                      ILogger<TLogger> logger, 
                                      IConfiguration configuration, 
                                      TDBContext databaseContext,
                                      IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check is not back office
            if (!this.Request.Method.Equals("OPTIONS") && !_viewModel.IsBackOffice())
            {
                // Obtain response model
                IOResponseModel responseModel = this.Error400("Restricted page.");

                // Override response
                context.Result = new JsonResult(responseModel);

                // Do nothing
                return;
            }
        }

        #endregion

        #region Client Methods

        [HttpPost]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null 
                || String.IsNullOrEmpty(requestModel.ClientDescription)) 
            {
                // Update response status
                this.Response.StatusCode = 400;

                // Create and return response
                return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid request data"), null);
            }

            // Obtain client info from view model
            IOClientBackOfficeInfoModel clientInfo = _viewModel.CreateClient(requestModel.ClientDescription, requestModel.RequestCount);

            // Create and return response
            return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfo);
		}

        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null)
			{
				// Update response status
				this.Response.StatusCode = 400;

				// Create and return response
				return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid request data"), null);
			}

            // Check delete client is success
            if (_viewModel.DeleteClient(requestModel.ClientId)) 
            {
                // Then create and return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return bad request
            this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            List<IOClientBackOfficeInfoModel> clientInfos = _viewModel.GetClients();

            // Create and return response
            return new IOClientListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
        }

        [HttpPost]
        public IOResponseModel UpdateClient([FromBody] IOClientUpdateRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Update response status
                this.Response.StatusCode = 400;

                // Create and return response
                return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid request data"), null);
            }

            // Check update client is success
            if (_viewModel.UpdateClient(requestModel.ClientId, requestModel.ClientDescription, requestModel.IsEnabled, requestModel.RequestCount, requestModel.MaxRequestCount))
            {
                // Then create and return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return bad request
            this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        #endregion

    }
}
