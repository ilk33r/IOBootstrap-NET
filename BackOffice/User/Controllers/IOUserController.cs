using System;
using IOBootstrap.NET.BackOffice.User.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    [IOBackoffice]
    public abstract class IOUserController<TViewModel> : IOBackOfficeController<TViewModel> where TViewModel : IIOUserViewModel, new()
    {
        #region Controller Lifecycle

        protected IOUserController(IConfiguration configuration, 
                                   IWebHostEnvironment environment, 
                                   ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region User Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost("[action]")]
        public virtual IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
			// Create and return response
            return ViewModel.AddUser(requestModel);
		}

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
		[HttpPost("[action]")]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
            // Check change password is success
            ViewModel.ChangePassword(requestModel.UserName, requestModel.OldPassword, requestModel.NewPassword);

			// Return response
			return new IOResponseModel();
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpGet("[action]")]
        public virtual IOListUserResponseModel ListUsers() 
        {
            // Obtain user list
            IList<IOUserInfoModel> users = ViewModel.ListUsers();

			// Create and return response
			return new IOListUserResponseModel(users);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost("[action]")]
        public IOUpdateUserResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
        {
            ViewModel.UpdateUser(requestModel);
            return new IOUpdateUserResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost("[action]")]
		public virtual IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel) 
        {
            ViewModel.DeleteUser(requestModel);
            return new IOResponseModel();
        }

        #endregion

    }
}
