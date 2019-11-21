using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOBackOfficeController<TLogger, TViewModel, TDBContext> : IOController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOBackOfficeViewModel<TDBContext>, new()
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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Update view model request value
            _viewModel._request = this.Request;

            // Check is not back office
            if (!this.Request.Method.Equals("OPTIONS") && !_viewModel.IsBackOffice())
            {
                // Update allow origin
                Response.Headers.Add("Access-Control-Allow-Origin", _configuration.GetValue<string>(IOConfigurationConstants.AllowedOrigin));
                Response.Headers.Add("Access-Control-Allow-Headers", Request.Headers["Access-Control-Request-Headers"]);

                // Obtain response model
                IOResponseModel responseModel = this.Error400("Restricted page.");

                // Override response
                JsonResult result = new JsonResult(responseModel);
                result.StatusCode = 400;
                context.Result = result;

                // Do nothing
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

        [IOUserRole(UserRoles.Admin)]
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

        [IOUserRole(UserRoles.Admin)]
        [HttpGet]
        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            List<IOClientBackOfficeInfoModel> clientInfos = _viewModel.GetClients();

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
