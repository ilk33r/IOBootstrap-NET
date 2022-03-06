using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOUserController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOUserViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOUserController(IConfiguration configuration, 
                                TDBContext databaseContext, 
                                IWebHostEnvironment environment, 
                                ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel)
        {
            IOAddUserResponseModel responseModel = ViewModel.AddUser(requestModel.UserName, requestModel.Password, requestModel.UserRole);

            if (responseModel == null)
            {
                return new IOAddUserResponseModel(IOResponseStatusMessages.UnkownException);
            }

            return responseModel;
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOMWFindRequestModel requestModel)
        {
            bool status = ViewModel.ChangePassword(requestModel.ID, requestModel.Where);

            if (!status)
            {
                return new IOResponseModel(IOResponseStatusMessages.UnkownException);
            }

            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOUserInfoModel> ListUsers([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<IOUserInfoModel> users = ViewModel.ListUsers();
            return new IOMWListResponseModel<IOUserInfoModel>(users);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
        {
            int status = ViewModel.UpdateUser(requestModel);
            return new IOResponseModel(status);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel)
        {
            int status = ViewModel.DeleteUser(requestModel);
            return new IOResponseModel(status);
        }
    }
}
