using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeController<TLogger, TViewModel>: IOController<TLogger, TViewModel>
        where TViewModel: IOBackOfficeViewModel, new()
    {

        #region Controller Lifecycle

        public IOBackOfficeController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
            : base(factory, logger, configuration, database)
        {
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check is not back office
            if (!_viewModel.IsBackOffice())
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
            IOClientBackOfficeInfoModel clientInfo = _viewModel.CreateClient(requestModel.ClientDescription);

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

        #endregion

    }
}
