using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.Net.Common.Messages.Users;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeAuthenticationController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeAuthenticationViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeAuthenticationController(IConfiguration configuration, 
                                                    TDBContext databaseContext, 
                                                    IWebHostEnvironment environment, 
                                                    ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [HttpPost]
        public IOMWUserResponseModel FindUserById([FromBody] IOMWFindRequestModel requestModel)
        {
            // Check if authentication result is true
            IOMWUserResponseModel? responseModel = ViewModel.FindUserById(requestModel.ID);

            if (responseModel == null)
            {
                return new IOMWUserResponseModel(IOResponseStatusMessages.UnkownException);
            }

            return responseModel;
        }
    }
}
