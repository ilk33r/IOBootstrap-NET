using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
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

        [IORequireHTTPS]
        [HttpPost]
        public IOMWUserResponseModel FindUserFromName([FromBody] IOMWFindRequestModel requestModel)
        {
            // Check if authentication result is true
            IOMWUserResponseModel responseModel = ViewModel.FindUserFromName(requestModel.Where);

            if (responseModel == null)
            {
                return new IOMWUserResponseModel(IOResponseStatusMessages.UnkownException);
            }

            return responseModel;
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWUserResponseModel FindUserById([FromBody] IOMWFindRequestModel requestModel)
        {
            // Check if authentication result is true
            IOMWUserResponseModel responseModel = ViewModel.FindUserById(requestModel.ID);

            if (responseModel == null)
            {
                return new IOMWUserResponseModel(IOResponseStatusMessages.UnkownException);
            }

            return responseModel;
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateUserToken([FromBody] IOMWUpdateTokenRequestModel requestModel)
        {
            ViewModel.UpdateUserToken(requestModel.ID, requestModel.UserToken, requestModel.TokenDate);
            return new IOResponseModel();
        }
    }
}
