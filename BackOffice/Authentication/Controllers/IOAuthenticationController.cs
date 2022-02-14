using System;
using IOBootstrap.NET.BackOffice.Authentication.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers
{
    [IOBackoffice]
    public abstract class IOAuthenticationController<TViewModel> : IOController<TViewModel> where TViewModel : IOAuthenticationViewModel, new()
    {
        #region Controller Lifecycle

        protected IOAuthenticationController(IConfiguration configuration, 
                                             IWebHostEnvironment environment, 
                                             ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region Authentication Api

        //TODO: Migrate with MW.
        /*
        [HttpPost]
        [IOValidateRequestModel]
        public IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
        {
            // Authenticate user
            Tuple<string, DateTimeOffset, string, int> authenticationResult = ViewModel.AuthenticateUser(requestModel.UserName, requestModel.Password);

            // Check if authentication result is true
            return new IOAuthenticationResponseModel(authenticationResult.Item1, authenticationResult.Item2, authenticationResult.Item3, authenticationResult.Item4);
        }
        */

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
