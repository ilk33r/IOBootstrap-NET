using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.Authentication.Models;
using IOBootstrap.NET.WebApi.Authentication.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace IOBootstrap.NET.WebApi.Authentication.Controllers
{
    public abstract class IOAuthenticationController<TLogger, TViewModel, TDBContext> : IOController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOAuthenticationViewModel<TDBContext>, new()
		where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOAuthenticationController(ILoggerFactory factory, 
                                          ILogger<TLogger> logger, 
                                          IConfiguration configuration, 
                                          TDBContext databaseContext,
                                          IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region Authentication Api

        [HttpPost]
        public IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || requestModel.ClientInfo == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), null, DateTime.Now);
            }

            // Check client 
            if (!_viewModel.CheckClient(requestModel.ClientInfo))
            {
                // Then return invalid clients
                this.Response.StatusCode = 400;
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), null, DateTime.Now);
            }

            // Authenticate user
            Tuple<bool, string, DateTimeOffset> authenticationResult = _viewModel.AuthenticateUser(requestModel.UserName, requestModel.Password);

            // Check if authentication result is true
            if (authenticationResult.Item1) {
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), authenticationResult.Item2, authenticationResult.Item3);
            }

            // Return response
            this.Response.StatusCode = 400;
            return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CREDIENTALS, "Invalid user."), authenticationResult.Item2, authenticationResult.Item3);
        }

        #endregion

    }
}
