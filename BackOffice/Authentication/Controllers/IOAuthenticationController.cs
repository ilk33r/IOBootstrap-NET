using System;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers
{
    [IOBackoffice]
    public abstract class IOAuthenticationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> 
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IIOAuthenticationViewModel<TDBContext>, new()
    {
        #region Controller Lifecycle

        protected IOAuthenticationController(IConfiguration configuration, 
                                             IWebHostEnvironment environment, 
                                             ILogger<IOLoggerType> logger,
                                             TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }

        #endregion

        #region Authentication Api

        [IOValidateRequestModel]
        [HttpPost("[action]")]
        public virtual IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
        {
            // Authenticate user
            Tuple<string, DateTimeOffset, string, int> authenticationResult = ViewModel.AuthenticateUser(requestModel.UserName, requestModel.Password);

            // Check if authentication result is true
            return new IOAuthenticationResponseModel(authenticationResult.Item1, authenticationResult.Item2, authenticationResult.Item3, authenticationResult.Item4);
        }

        [IOValidateRequestModel]
        [HttpPost("[action]")]
        public virtual IOCheckTokenResponseModel CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
        {
            // Check token
            Tuple<DateTimeOffset, string, int> checkTokenResult = ViewModel.CheckToken(requestModel.Token);

            // Check if authentication result is true
            return new IOCheckTokenResponseModel(checkTokenResult.Item1, checkTokenResult.Item2, checkTokenResult.Item3);
        }
        
        #endregion
    }
}
