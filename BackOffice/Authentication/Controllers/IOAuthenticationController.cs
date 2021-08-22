using System;
using IOBootstrap.NET.BackOffice.Authentication.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers
{
    public abstract class IOAuthenticationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> where TViewModel : IOAuthenticationViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        protected IOAuthenticationController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Authentication Api

        [HttpPost]
        [IOValidateRequestModel]
        public IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
        {
            // Authenticate user
            Tuple<string, DateTimeOffset, string, int> authenticationResult = ViewModel.AuthenticateUser(requestModel.UserName, requestModel.Password);

            // Check if authentication result is true
            return new IOAuthenticationResponseModel(authenticationResult.Item1, authenticationResult.Item2, authenticationResult.Item3, authenticationResult.Item4);
        }

        [HttpPost]
        [IOValidateRequestModel]
        public IOCheckTokenResponseModel CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
        {
            // Check token
            Tuple<DateTimeOffset, string, int> checkTokenResult = ViewModel.CheckToken(requestModel.Token);

            // Check if authentication result is true
            return new IOCheckTokenResponseModel(checkTokenResult.Item1, checkTokenResult.Item2, checkTokenResult.Item3);
        }

        #endregion

    }
}
