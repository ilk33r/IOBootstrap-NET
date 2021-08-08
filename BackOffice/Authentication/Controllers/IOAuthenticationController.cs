﻿using System;
using IOBootstrap.NET.BackOffice.Authentication.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Common.Models.Shared;
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
            Tuple<bool, string, DateTimeOffset, string, int> authenticationResult = ViewModel.AuthenticateUser(requestModel.UserName, requestModel.Password);

            // Check if authentication result is true
            if (authenticationResult.Item1) {
                return new IOAuthenticationResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), authenticationResult.Item2, authenticationResult.Item3, authenticationResult.Item4, authenticationResult.Item5);
            }

            // Return response
            throw new IOInvalidCredentialsException();
        }

        [HttpPost]
        [IOValidateRequestModel]
        public IOCheckTokenResponseModel CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
        {
            // Check token
            Tuple<bool, DateTimeOffset, string, int> checkTokenResult = ViewModel.CheckToken(requestModel.Token);

            // Check if authentication result is true
            if (checkTokenResult.Item1)
            {
                return new IOCheckTokenResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), checkTokenResult.Item2, checkTokenResult.Item3, checkTokenResult.Item4);
            }

            // Return response
            throw new IOInvalidCredentialsException();
        }

        #endregion

    }
}
