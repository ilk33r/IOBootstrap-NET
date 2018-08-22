using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.WebApi.BackOffice.Controllers;
using IOBootstrap.NET.WebApi.User.Models;
using IOBootstrap.NET.WebApi.User.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    public abstract class IOUserController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOUserViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOUserController(ILoggerFactory factory, 
                                ILogger<TLogger> logger, 
                                IConfiguration configuration, 
                                TDBContext databaseContext,
                                IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region User Methods

        [HttpPost]
        public IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new IOAddUserResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), 0, null);
			}

            // Obtain add user response
            Tuple<bool, int, string> addUserStatus = _viewModel.AddUser(requestModel.UserName, requestModel.Password, requestModel.UserRole);

            // Check add user is success
            if (addUserStatus.Item1) 
            {
				// Create and return response
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), addUserStatus.Item2, addUserStatus.Item3);
            }

			// Return user exists response
			this.Response.StatusCode = 400;
			return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User exists"), 0, null);
		}

		[HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.OldPassword)
                || String.IsNullOrEmpty(requestModel.NewPassword)
                || requestModel.NewPassword.Length < 4)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return error400;
			}

            // Check change password is success
            if (_viewModel.ChangePassword(requestModel.UserName, requestModel.OldPassword, requestModel.NewPassword))
            {
				// Return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return response
			this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid user."));
        }

        [HttpPost]
        public IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return error400;
			}

            // Obtain user entity
            IOUserEntity userEntity = _databaseContext.Users.Find(requestModel.UserId);

			// Check user entity is not null
			if (userEntity != null)
			{
                // Delete all entity
                _databaseContext.Remove(userEntity);
                _databaseContext.SaveChanges();

				// Then return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
			}

			// Return bad request
			this.Response.StatusCode = 400;
			return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User not found."));
        }

        [HttpGet]
        public IOListUserResponseModel ListUsers() 
        {
            // Obtain user list
            List<IOUserInfoModel> users = _viewModel.ListUsers();

			// Create and return response
			return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), users);
        }

        #endregion

    }
}
